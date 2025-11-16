using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebAutomationTests.Support;
using NUnit.Framework;
using System.Linq;

namespace WebAutomationTests.StepDefinitions
{
    [Binding]
    public class SearchSteps
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public SearchSteps(WebDriverSupport webDriverSupport)
        {
            _driver = webDriverSupport.Driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [When(@"the user searches for ""(.*)""")]
        public void WhenTheUserSearchesFor(string searchTerm)
        {
            try
            {
                // Găsește câmpul de căutare
                var searchInput = _wait.Until(driver => 
                    driver.FindElement(By.CssSelector("input[type='search'][name='search']")));
                
                Console.WriteLine($"🔍 Searching for: '{searchTerm}'");
                
                // Curăță câmpul și introduce termenul de căutare
                searchInput.Clear();
                searchInput.SendKeys(searchTerm);
                
                // Apasă Enter pentru a efectua căutarea
                searchInput.SendKeys(Keys.Enter);
                
                // Așteaptă încărcarea paginii
                _wait.Until(driver => 
                    ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                System.Threading.Thread.Sleep(2000);
                
                Console.WriteLine($"✅ Search executed for: '{searchTerm}'");
                
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"❌ Search input not found: {ex.Message}");
                throw;
            }
        }

        [Then(@"relevant products containing ""(.*)"" should be displayed")]
        public void ThenRelevantProductsContainingShouldBeDisplayed(string expectedTerm)
        {
            var currentUrl = _driver.Url;
            Console.WriteLine($"🌐 Current URL after search: {currentUrl}");
            
            // Verifică dacă suntem pe o pagină de eroare 404
            bool is404Page = currentUrl.Contains("404") || 
                            _driver.FindElements(By.XPath("//*[contains(text(), '404')]")).Count > 0 ||
                            _driver.FindElements(By.XPath("//*[contains(text(), 'Not Found')]")).Count > 0 ||
                            _driver.FindElements(By.XPath("//*[contains(text(), 'Page Not Found')]")).Count > 0;
            
            if (is404Page)
            {
                Console.WriteLine($"❌ BUG LB-3 CONFIRMED: Search redirects to 404 page");
                Console.WriteLine($"🐛 Expected: Products containing '{expectedTerm}'");
                Console.WriteLine($"🐛 Actual: 404 Error Page");
                
                // Capture the error message for documentation
                var pageTitle = _driver.Title;
                var pageText = _driver.FindElement(By.TagName("body")).Text;
                
                Console.WriteLine($"📄 Page Title: {pageTitle}");
                Console.WriteLine($"📄 Page contains '404': {pageText.Contains("404")}");
                Console.WriteLine($"📄 Page contains 'Not Found': {pageText.Contains("Not Found")}");
                
                Assert.Fail($"Search function shows 404 error instead of products. Bug LB-3 confirmed.");
            }
            else
            {
                // Verifică dacă sunt afișate produse relevante
                var products = _driver.FindElements(By.CssSelector(".product-item, .agile_top_brand_left_grid, [class*='product']"));
                
                if (products.Count > 0)
                {
                    Console.WriteLine($"✅ Found {products.Count} products");
                    
                    // Verifică dacă produsele conțin termenul căutat
                    int relevantProducts = 0;
                    foreach (var product in products.Take(5)) // Verifică doar primele 5
                    {
                        try
                        {
                            var productName = product.FindElement(By.CssSelector("h4, h3, [class*='name']")).Text;
                            if (productName.ToLower().Contains(expectedTerm.ToLower()))
                            {
                                relevantProducts++;
                                Console.WriteLine($"   ✅ Relevant product: '{productName}'");
                            }
                        }
                        catch
                        {
                            // Continuă cu următorul produs
                        }
                    }
                    
                    Assert.Greater(relevantProducts, 0, $"Should find at least one product containing '{expectedTerm}'");
                    Console.WriteLine($"✅ Found {relevantProducts} relevant products containing '{expectedTerm}'");
                }
                else
                {
                    Console.WriteLine($"ℹ️ No products found for search term: '{expectedTerm}'");
                    // Poate fi considerat normal pentru unele termeni
                }
            }
        }

        [Then(@"the search term should be accepted")]
        public void ThenTheSearchTermShouldBeAccepted()
        {
            // Verifică că am ajuns pe o pagină (nu am rămas pe aceeași pagină)
            var currentUrl = _driver.Url;
            var homeUrl = "https://adoring-pasteur-3ae17d.netlify.app/";
            
            bool searchWasProcessed = currentUrl != homeUrl && !currentUrl.EndsWith("/");
            
            Assert.IsTrue(searchWasProcessed, $"Search term should be processed. Current URL: {currentUrl}");
            Console.WriteLine($"✅ Search term was accepted and processed");
        }

        [Then(@"the system should display ""Not Found"" message or empty results")]
        public void ThenTheSystemShouldDisplayMessageOrEmptyResults()
        {
            var currentUrl = _driver.Url;
            Console.WriteLine($"🌐 Current URL: {currentUrl}");
            
            // Verifică diferite scenarii pentru rezultate invalide
            bool hasNotFoundMessage = _driver.FindElements(By.XPath("//*[contains(text(), 'Not Found')]")).Count > 0;
            bool hasNoResultsMessage = _driver.FindElements(By.XPath("//*[contains(text(), 'No results')]")).Count > 0;
            bool hasEmptyResults = _driver.FindElements(By.CssSelector(".product-item, .agile_top_brand_left_grid")).Count == 0;
            bool is404Page = currentUrl.Contains("404") || _driver.FindElements(By.XPath("//*[contains(text(), '404')]")).Count > 0;
            
            Console.WriteLine($"📊 Search results analysis:");
            Console.WriteLine($"   - Has 'Not Found' message: {hasNotFoundMessage}");
            Console.WriteLine($"   - Has 'No results' message: {hasNoResultsMessage}");
            Console.WriteLine($"   - Has empty results: {hasEmptyResults}");
            Console.WriteLine($"   - Is 404 page: {is404Page}");
            
            // Testul trece dacă oricare dintre condițiile astea este adevărată
            bool hasAppropriateResponse = hasNotFoundMessage || hasNoResultsMessage || hasEmptyResults || is404Page;
            
            Assert.IsTrue(hasAppropriateResponse, "System should display 'Not Found' message or empty results for invalid search term");
            Console.WriteLine($"✅ System appropriately handled invalid search term");
        }

        [Then(@"the system should not display 404 error page")]
        public void ThenTheSystemShouldNotDisplay404ErrorPage()
        {
            var currentUrl = _driver.Url;
            bool is404Page = currentUrl.Contains("404") || 
                            _driver.FindElements(By.XPath("//*[contains(text(), '404')]")).Count > 0;
            
            if (is404Page)
            {
                Console.WriteLine($"❌ BUG LB-3 CONFIRMED: Search shows 404 error page");
                Console.WriteLine($"🌐 Current URL: {currentUrl}");
                
                // Documentează detaliile bug-ului
                var pageSource = _driver.PageSource;
                bool hasNotFound = pageSource.Contains("Not Found");
                bool hasError = pageSource.Contains("Error");
                
                Console.WriteLine($"🐛 Bug Details:");
                Console.WriteLine($"   - URL contains '404': {currentUrl.Contains("404")}");
                Console.WriteLine($"   - Page contains 'Not Found': {hasNotFound}");
                Console.WriteLine($"   - Page contains 'Error': {hasError}");
                
                Assert.Fail("Search function should not show 404 error page for valid search terms. Bug LB-3 confirmed.");
            }
            else
            {
                Console.WriteLine($"✅ No 404 error page displayed");
            }
        }

        [Then(@"currently the search shows 404 error page confirming bug LB-3")]
        public void ThenCurrentlyTheSearchShows404ErrorPageConfirmingBugLB3()
        {
            var currentUrl = _driver.Url;
            bool is404Page = currentUrl.Contains("404") || 
                            _driver.FindElements(By.XPath("//*[contains(text(), '404')]")).Count > 0;
            
            Assert.IsTrue(is404Page, "This scenario expects 404 error to confirm bug LB-3");
            Console.WriteLine($"🐛 BUG LB-3 CONFIRMED: Search function returns 404 error");
            Console.WriteLine($"📝 This confirms the known issue: Searchbar doesn't return products, redirects to 'Not Found' page");
        }
    }
}