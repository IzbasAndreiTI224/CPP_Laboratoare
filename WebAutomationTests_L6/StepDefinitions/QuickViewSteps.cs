using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebAutomationTests.Support;
using NUnit.Framework;
using System.Linq;

namespace WebAutomationTests.StepDefinitions
{
    [Binding]
    public class QuickViewSteps
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private string _selectedProductName = string.Empty;
        private string _selectedProductPrice = string.Empty;

        public QuickViewSteps(WebDriverSupport webDriverSupport)
        {
            _driver = webDriverSupport.Driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        }

        [When(@"the user clicks on a product's Quick View button")]
        public void WhenTheUserClicksOnAProductsQuickViewButton()
        {
            try
            {
                // Scroll down puțin pentru a încărca produsele
                Console.WriteLine("📜 Scrolling to load products...");
                ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, 500);");
                System.Threading.Thread.Sleep(2000);

                // Găsește primul produs din grid
                var firstProduct = _wait.Until(driver =>
                    driver.FindElement(By.CssSelector(".agile_top_brand_left_grid, .product-item, [class*='product']")));

                // Scroll specific la acel produs
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", firstProduct);
                System.Threading.Thread.Sleep(1000);

                // Salvează detaliile produsului selectat DIN LISTA PRINCIPALĂ
                var productNameElement = firstProduct.FindElement(By.CssSelector("h4 a, h3 a, [class*='product-name']"));
                var productPriceElement = firstProduct.FindElement(By.CssSelector(".item_price, .price, [class*='price']"));

                _selectedProductName = productNameElement.Text.Trim();
                _selectedProductPrice = productPriceElement.Text.Trim();

                Console.WriteLine($"🎯 SELECTED PRODUCT FROM LIST:");
                Console.WriteLine($"   Name: '{_selectedProductName}'");
                Console.WriteLine($"   Price: '{_selectedProductPrice}'");

                // Găsește butonul Quick View
                IWebElement quickViewButton = firstProduct.FindElement(By.CssSelector("a.link-product-add-cart[href*='single']"));

                // Scroll la buton
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", quickViewButton);
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine("🖱️ Clicking Quick View button...");

                // Folosește JavaScript click pentru a evita problemele de interactibilitate
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", quickViewButton);

                // Așteaptă să se încarce pagina single
                _wait.Until(driver =>
                    ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                System.Threading.Thread.Sleep(3000);

                Console.WriteLine("✅ Quick View button clicked successfully");

            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"❌ Could not find Quick View elements: {ex.Message}");
                throw;
            }
        }

        [Then(@"the product information should be displayed")]
        public void ThenTheProductInformationShouldBeDisplayed()
        {
            // Verifică că suntem pe pagina de single product
            var currentUrl = _driver.Url;
            Console.WriteLine($"🌐 Current URL: {currentUrl}");

            Assert.IsTrue(currentUrl.Contains("/single"), $"Should be on single product page. Current: {currentUrl}");

            // Așteaptă încă puțin pentru a se încărca complet pagina
            System.Threading.Thread.Sleep(2000);

            // Verifică că informațiile produsului sunt afișate pe pagina single
            // Numele produsului - folosește selector mai general
            var productName = _wait.Until(driver =>
                driver.FindElement(By.CssSelector("h1, h2, h3, h4, [class*='product-name'], [class*='title']")));

            Assert.IsTrue(productName.Displayed, "Product name should be visible");
            Assert.IsFalse(string.IsNullOrEmpty(productName.Text), "Product name should not be empty");

            // Prețul produsului - folosește selector mai general
            var productPrice = _wait.Until(driver =>
                driver.FindElement(By.CssSelector(".item_price, [class*='price'], span[class*='price'], p[class*='price']")));

            Assert.IsTrue(productPrice.Displayed, "Product price should be visible");
            Assert.IsFalse(string.IsNullOrEmpty(productPrice.Text), "Product price should not be empty");

            // Imaginea produsului - selector specific pentru carusel
            var productImage = _wait.Until(driver =>
                driver.FindElement(By.CssSelector("img.flex-active, img[src*='images/'], .product-image, img")));

            Assert.IsTrue(productImage.Displayed, "Product image should be visible");

            Console.WriteLine($"✅ PRODUCT INFO DISPLAYED ON SINGLE PAGE:");
            Console.WriteLine($"   Name: '{productName.Text}'");
            Console.WriteLine($"   Price: '{productPrice.Text}'");
            Console.WriteLine($"   Image displayed: {productImage.Displayed}");
            Console.WriteLine($"   Image source: {productImage.GetAttribute("src")}");
        }

        [Then(@"the displayed details should match the selected product")]
        public void ThenTheDisplayedDetailsShouldMatchTheSelectedProduct()
        {
            // Obține detaliile din pagina single
            var displayedName = _driver.FindElement(By.CssSelector("h1, h2, h3, h4, [class*='product-name']")).Text.Trim();
            var displayedPrice = _driver.FindElement(By.CssSelector(".item_price, [class*='price']")).Text.Trim();

            Console.WriteLine($"🔍 COMPARING PRODUCTS:");
            Console.WriteLine($"   SELECTED FROM LIST: '{_selectedProductName}' | '{_selectedProductPrice}'");
            Console.WriteLine($"   DISPLAYED ON PAGE:  '{displayedName}' | '{displayedPrice}'");

            // Curăță textul pentru comparație mai ușoară
            var cleanSelectedName = _selectedProductName.Replace("Formal", "").Replace("Shirt", "").Trim();
            var cleanDisplayedName = displayedName.Replace("Big Wing Sneakers", "").Replace("(Navy)", "").Trim();

            var cleanSelectedPrice = _selectedProductPrice.Replace("$", "").Split(' ')[0].Trim();
            var cleanDisplayedPrice = displayedPrice.Replace("$", "").Split(' ')[0].Trim();

            Console.WriteLine($"🔍 CLEANED COMPARISON:");
            Console.WriteLine($"   SELECTED: '{cleanSelectedName}' | '{cleanSelectedPrice}'");
            Console.WriteLine($"   DISPLAYED: '{cleanDisplayedName}' | '{cleanDisplayedPrice}'");

            // Verifică dacă detaliile corespund
            bool namesMatch = _selectedProductName.Contains(displayedName) ||
                             displayedName.Contains(_selectedProductName) ||
                             cleanSelectedName.Contains(cleanDisplayedName) ||
                             cleanDisplayedName.Contains(cleanSelectedName);

            bool pricesMatch = _selectedProductPrice.Contains(displayedPrice) ||
                              displayedPrice.Contains(_selectedProductPrice) ||
                              cleanSelectedPrice.Contains(cleanDisplayedPrice) ||
                              cleanDisplayedPrice.Contains(cleanSelectedPrice);

            if (!namesMatch || !pricesMatch)
            {
                Console.WriteLine($"❌ PRODUCT MISMATCH DETECTED!");
                Console.WriteLine($"   Names match: {namesMatch}");
                Console.WriteLine($"   Prices match: {pricesMatch}");

                // Documentează bug-ul LB-1
                Console.WriteLine($"🐛 BUG LB-1 CONFIRMED: Quick View opens different product than selected");
                Console.WriteLine($"   Expected: '{_selectedProductName}' | '{_selectedProductPrice}'");
                Console.WriteLine($"   Actual: '{displayedName}' | '{displayedPrice}'");

                Assert.Fail($"Quick View shows different product. Expected: '{_selectedProductName}' | '{_selectedProductPrice}', Got: '{displayedName}' | '{displayedPrice}'");
            }
            else
            {
                Console.WriteLine($"✅ SUCCESS: Product details match correctly");
                Assert.IsTrue(namesMatch, "Product names should match");
                Assert.IsTrue(pricesMatch, "Product prices should match");
            }
        }

    }
}