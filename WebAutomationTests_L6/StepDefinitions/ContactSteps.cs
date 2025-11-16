using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebAutomationTests.Support;
using NUnit.Framework;
using System.Linq;

namespace WebAutomationTests.StepDefinitions
{
    [Binding]
    public class ContactSteps
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public ContactSteps(WebDriverSupport webDriverSupport)
        {
            _driver = webDriverSupport.Driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [When(@"the user navigates to the Contact page")]
        public void WhenTheUserNavigatesToTheContactPage()
        {
            try
            {
                // Găsește și apasă link-ul Contact din meniu
                var contactLink = _wait.Until(driver =>
                    driver.FindElement(By.CssSelector("a.menu__link[href*='contact']")));

                Console.WriteLine("📞 Navigating to Contact page...");
                contactLink.Click();

                // Așteaptă încărcarea paginii
                _wait.Until(driver =>
                    ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                System.Threading.Thread.Sleep(2000);

                Console.WriteLine("✅ Successfully navigated to Contact page");

            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"❌ Contact link not found: {ex.Message}");
                throw;
            }
        }

        [Then(@"the URL should change to ""/contact""")]
        public void ThenTheURLShouldChangeToContact()
        {
            var currentUrl = _driver.Url;
            bool isOnContactPage = currentUrl.Contains("/contact");

            Assert.IsTrue(isOnContactPage, $"Should be on contact page. Current URL: {currentUrl}");
            Console.WriteLine($"✅ URL changed to contact page: {currentUrl}");
        }

        [Then(@"the contact form should be visible with required fields")]
        public void ThenTheContactFormShouldBeVisibleWithRequiredFields()
        {
            // Verifică secțiunea cu informațiile de contact
            var contactInfoSection = _wait.Until(driver =>
                driver.FindElement(By.CssSelector(".address-grid, [class*='address'], [class*='contact-info']")));

            Assert.IsTrue(contactInfoSection.Displayed, "Contact information section should be visible");
            Console.WriteLine("✅ Contact information section is visible");

            // Verifică informațiile de contact: telefon, email, adresă
            var phoneElement = contactInfoSection.FindElements(By.XPath(".//*[contains(text(), 'Telephone')]//following-sibling::span"));
            var emailElement = contactInfoSection.FindElements(By.XPath(".//*[contains(text(), 'Mail')]//following-sibling::a"));
            var addressElement = contactInfoSection.FindElements(By.XPath(".//*[contains(text(), 'Location')]//following-sibling::span"));

            bool hasPhone = phoneElement.Count > 0;
            bool hasEmail = emailElement.Count > 0;
            bool hasAddress = addressElement.Count > 0;

            Console.WriteLine($"📋 Contact information found:");
            Console.WriteLine($"   - Telephone: {hasPhone}");
            Console.WriteLine($"   - Email: {hasEmail}");
            Console.WriteLine($"   - Address: {hasAddress}");

            // Afișează valorile găsite
            if (hasPhone)
            {
                Console.WriteLine($"   📞 Telephone: {phoneElement.First().Text}");
            }
            if (hasEmail)
            {
                Console.WriteLine($"   📧 Email: {emailElement.First().Text}");
            }
            if (hasAddress)
            {
                Console.WriteLine($"   📍 Address: {addressElement.First().Text}");
            }

            // Verifică că toate cele 3 informații sunt prezente
            Assert.IsTrue(hasPhone, "Contact page should have telephone information");
            Assert.IsTrue(hasEmail, "Contact page should have email information");
            Assert.IsTrue(hasAddress, "Contact page should have address information");

            Console.WriteLine("✅ All contact information is present and correct");
        }

        [Then(@"the home page should be displayed correctly")]
        public void ThenTheHomePageShouldBeDisplayedCorrectly()
        {
            var currentUrl = _driver.Url;
            bool isOnHomePage = currentUrl.Contains("adoring-pasteur-3ae17d.netlify.app") &&
                               (currentUrl.EndsWith("/") || currentUrl.Contains("/?"));

            Assert.IsTrue(isOnHomePage, $"Should be on home page. Current: {currentUrl}");

            // Verifică elementele specifice homepage-ului
            var mainMenu = _driver.FindElement(By.CssSelector("ul.navbar-nav.menu__list"));
            Assert.IsTrue(mainMenu.Displayed, "Main menu should be visible");

            Console.WriteLine($"✅ Successfully returned to home page: {currentUrl}");
            Console.WriteLine($"✅ Main menu is visible: {mainMenu.Displayed}");
        }
    }
}