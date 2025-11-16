using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Microsoft.Extensions.Configuration;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace GoogleTests.Support
{
    public class Driver
    {
        private static IWebDriver _driver;
        private static IConfiguration _configuration;

        public static IWebDriver Current => _driver ??= CreateDriver();

        public static void Initialize()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private static IWebDriver CreateDriver()
        {
            var browserStackEnabled = bool.Parse(_configuration["BrowserStack:Enabled"] ?? "false");
            return browserStackEnabled ? CreateBrowserStackDriver() : CreateLocalDriver();
        }

        private static IWebDriver CreateLocalDriver()
        {
            // Această linie descarcă AUTOMAT driver-ul corect pentru Chrome
            new DriverManager().SetUpDriver(new ChromeConfig());

            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            try
            {
                return new ChromeDriver(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChromeDriver failed: {ex.Message}");
                throw;
            }
        }

        private static IWebDriver CreateBrowserStackDriver()
        {
            var options = new ChromeOptions();
            options.AddAdditionalOption("browserName", "Chrome");
            options.AddAdditionalOption("browserVersion", "latest");

            var browserStackOptions = new Dictionary<string, object>
            {
                ["userName"] = _configuration["BrowserStack:Username"] ?? "",
                ["accessKey"] = _configuration["BrowserStack:AccessKey"] ?? "",
                ["os"] = "Windows",
                ["osVersion"] = "11"
            };

            options.AddAdditionalOption("bstack:options", browserStackOptions);

            return new RemoteWebDriver(
                new Uri($"https://{_configuration["BrowserStack:Server"]}/wd/hub"),
                options.ToCapabilities(),
                TimeSpan.FromSeconds(60)
            );
        }

        public static void Close()
        {
            _driver?.Quit();
            _driver = null;
        }
    }
}