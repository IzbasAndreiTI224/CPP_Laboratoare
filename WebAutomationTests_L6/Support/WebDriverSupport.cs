using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;

namespace WebAutomationTests.Support
{
    [Binding]
    public class WebDriverSupport
    {
        private readonly IWebDriver _driver;

        public WebDriverSupport()
        {
            // Configurează Chrome options
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.AddArgument("--disable-notifications");

            _driver = new ChromeDriver(chromeOptions);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        public IWebDriver Driver => _driver;

        [AfterScenario]
        public void AfterScenario()
        {
            _driver.Quit();
        }
    }
}