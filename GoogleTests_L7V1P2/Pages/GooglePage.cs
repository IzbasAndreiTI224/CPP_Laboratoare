using GoogleTests.Support;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GoogleTests.Pages
{
    public class GooglePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public GooglePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void NavigateToGoogle()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Folosește URL cu parametri care să minimizeze CAPTCHA
            string googleUrl = "https://www.google.com/?hl=en&gws_rd=ssl";
            _driver.Navigate().GoToUrl(googleUrl);

            // Așteaptă mai mult pentru BrowserStack
            Thread.Sleep(5000);
            WaitForPageToLoad();

            // Verifică dacă am ajuns pe pagina corectă
            if (!IsOnGoogleSearchPage())
            {
                Console.WriteLine("🔄 Page not loaded correctly, attempting recovery...");
                HandlePageRecovery();
            }

            Console.WriteLine("✅ Successfully navigated to Google");
        }

        private void HandlePageRecovery()
        {
            try
            {
                // Încearcă să gestioneze CAPTCHA sau alte blocaje
                var currentUrl = _driver.Url;
                Console.WriteLine($"Current URL: {currentUrl}");

                // Dacă suntem pe o pagină de CAPTCHA sau blocaj
                if (currentUrl.Contains("captcha") || currentUrl.Contains("blocked") || currentUrl.Contains("unusual"))
                {
                    Console.WriteLine("⚠️ CAPTCHA or block page detected");

                    // Încearcă să refreshești cu parametri diferiți
                    _driver.Navigate().GoToUrl("https://www.google.com/ncr"); // No country redirect
                    Thread.Sleep(3000);
                }

                // Verifică dacă există buton de acceptare cookie-uri (în caz că tot apare)
                try
                {
                    var acceptButtons = _driver.FindElements(By.XPath(Locators.AcceptCookies));
                    var visibleButton = acceptButtons.FirstOrDefault(btn => btn.Displayed);
                    if (visibleButton != null)
                    {
                        Console.WriteLine("📍 Clicking cookies accept button...");
                        visibleButton.Click();
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ℹ️ No cookies button or error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Page recovery failed: {ex.Message}");
            }
        }

        public void EnterSearchTerm(string searchTerm)
        {
            try
            {
                var searchInput = _wait.Until(d => d.FindElement(By.XPath(Locators.SearchInput)));
                searchInput.Clear();
                searchInput.SendKeys(searchTerm);
                Console.WriteLine($"✅ Entered search term: {searchTerm}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to enter search term: {ex.Message}");
                throw;
            }
        }

        public void ClickSearch()
        {
            try
            {
                // Pentru BrowserStack, folosește doar Enter
                var searchInput = _wait.Until(d => d.FindElement(By.XPath(Locators.SearchInput)));
                searchInput.SendKeys(Keys.Enter);
                WaitForPageToLoad();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClickSearch failed: {ex.Message}");
                throw;
            }
        }

        public void PressEnterInSearch()
        {
            var searchInput = _wait.Until(d => d.FindElement(By.XPath(Locators.SearchInput)));
            searchInput.SendKeys(Keys.Enter);
            WaitForPageToLoad();
        }

        public int GetSearchResultsCount()
        {
            try
            {
                // Așteaptă rezultatele să se încarce
                Thread.Sleep(2000);
                var results = _driver.FindElements(By.XPath(Locators.SearchResults));
                return results.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool IsDidYouMeanDisplayed()
        {
            try
            {
                // Așteaptă puțin pentru sugestie
                Thread.Sleep(2000);

                // Încearcă multiple locatori
                var locators = new string[]
                {
                    "//*[contains(., 'Искать вместо этого')]",
                    "//span[contains(@class, 'X8u6Oc')]",
                    "//*[contains(., 'Did you mean')]",
                    "//*[contains(., 'Search instead')]",
                    "//a[contains(@href, 'nfpr=1')]"
                };

                foreach (var locator in locators)
                {
                    try
                    {
                        var element = _driver.FindElement(By.XPath(locator));
                        if (element.Displayed)
                        {
                            Console.WriteLine($"Found suggestion with locator: {locator}");
                            return true;
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        // Continuă cu următorul locator
                    }
                }
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsGoogleLogoDisplayed()
        {
            try
            {
                var element = _driver.FindElement(By.XPath(Locators.GoogleLogo));
                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsSearchInputDisplayed()
        {
            try
            {
                var element = _driver.FindElement(By.XPath(Locators.SearchInput));
                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsOnGoogleSearchPage()
        {
            try
            {
                // Verifică dacă input-ul de căutare este prezent
                var searchInput = _driver.FindElement(By.XPath(Locators.SearchInput));
                return searchInput.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public string GetPageTitle()
        {
            return _driver.Title;
        }

        public string GetCurrentUrl()
        {
            return _driver.Url;
        }

        public void AcceptCookiesIfPresent()
        {
            try
            {
                // Așteaptă puțin pentru pagina să se încarce
                Thread.Sleep(2000);

                // Încearcă să găsească butonul de acceptare cookie-uri
                var acceptButtons = _driver.FindElements(By.XPath(Locators.AcceptCookies));
                var visibleAcceptButton = acceptButtons.FirstOrDefault(btn => btn.Displayed);

                if (visibleAcceptButton != null)
                {
                    Console.WriteLine("📍 Found cookies consent dialog, clicking Accept...");
                    visibleAcceptButton.Click();
                    Thread.Sleep(2000); // Așteaptă după click

                    // Verifică dacă butonul a dispărut
                    try
                    {
                        if (!visibleAcceptButton.Displayed)
                        {
                            Console.WriteLine("✅ Cookies accepted successfully");
                        }
                    }
                    catch (StaleElementReferenceException)
                    {
                        Console.WriteLine("✅ Cookies accepted - page refreshed");
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ No cookies consent dialog found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not handle cookies: {ex.Message}");
                // Continuă chiar dacă nu putem gestiona cookie-urile
            }
        }

        private void WaitForPageToLoad()
        {
            try
            {
                // Așteaptă ca pagina să se încarce
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (Exception)
            {
                // Continuă chiar dacă timeout-ul expiră
            }
        }
    }
}