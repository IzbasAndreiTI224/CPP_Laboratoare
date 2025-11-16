using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebAutomationTests.Support;
using NUnit.Framework;

namespace WebAutomationTests.StepDefinitions
{
    [Binding]
    public class CommonSteps
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public CommonSteps(WebDriverSupport webDriverSupport)
        {
            _driver = webDriverSupport.Driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Given(@"the user is on the home page")]
        public void GivenTheUserIsOnTheHomePage()
        {
            _driver.Navigate().GoToUrl("https://adoring-pasteur-3ae17d.netlify.app/");
            _wait.Until(driver =>
                ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            System.Threading.Thread.Sleep(2000);

            Console.WriteLine($"✅ User is on home page: {_driver.Url}");
        }

        [When(@"the user navigates back to home page")]
        public void WhenTheUserNavigatesBackToHomePage()
        {
            Console.WriteLine("🏠 Navigating back to home page...");
            _driver.Navigate().GoToUrl("https://adoring-pasteur-3ae17d.netlify.app/");

            _wait.Until(driver =>
                ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            System.Threading.Thread.Sleep(2000);
        }

        [Then(@"the user should be redirected back to the main page")]
        public void ThenTheUserShouldBeRedirectedBackToTheMainPage()
        {
            var currentUrl = _driver.Url;
            bool isOnHomePage = currentUrl.Contains("adoring-pasteur-3ae17d.netlify.app") &&
                               (currentUrl.EndsWith("/") || currentUrl.Contains("/?"));

            Assert.IsTrue(isOnHomePage,
                $"Should be redirected to main page. Current: {currentUrl}");

            // Verifică că elementele homepage sunt prezente
            var mainMenu = _driver.FindElement(By.CssSelector("ul.navbar-nav.menu__list"));
            Assert.IsTrue(mainMenu.Displayed, "Main menu should be visible");

            Console.WriteLine($"✅ Successfully returned to main page: {currentUrl}");
        }
    }
}