using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
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

            if (browserStackEnabled)
            {
                return CreateBrowserStackDriver();
            }
            else
            {
                return CreateLocalDriver();
            }
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

            // CONFIGURAȚII AVANSATE ANTI-DETECTARE ȘI ANTI-CAPTCHA
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--allow-running-insecure-content");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-plugins");
            options.AddArgument("--disable-background-timer-throttling");
            options.AddArgument("--disable-backgrounding-occluded-windows");
            options.AddArgument("--disable-renderer-backgrounding");

            // User agent real pentru a părea ca browser normal
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            // Exclude automation features
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            // CONFIGURAȚII BROWSERSTACK CU PROTEȚIE ANTI-CAPTCHA
            var browserStackOptions = new Dictionary<string, object>
            {
                ["userName"] = _configuration["BrowserStack:Username"],
                ["accessKey"] = _configuration["BrowserStack:AccessKey"],
                ["os"] = "Windows",
                ["osVersion"] = "11",
                ["browserName"] = "Chrome",
                ["browserVersion"] = "latest",
                ["projectName"] = _configuration["BrowserStack:ProjectName"] ?? "Google Tests",
                ["buildName"] = _configuration["BrowserStack:BuildName"] ?? "Automated Tests",
                ["sessionName"] = "Google Search Tests",
                ["local"] = _configuration["BrowserStack:Local"] ?? "false",
                ["seleniumVersion"] = "4.8.0",
                ["resolution"] = "1920x1080",
                ["maskCommands"] = "setValues, getValues, setCookies, getCookies", // Ascunde comenzile Selenium
                ["acceptInsecureCerts"] = "true",
                ["timeouts"] = new Dictionary<string, object>
                {
                    ["implicit"] = 30000,
                    ["pageLoad"] = 30000,
                    ["script"] = 30000
                }
            };

            options.AddAdditionalOption("bstack:options", browserStackOptions);

            try
            {
                Console.WriteLine("🚀 Starting BrowserStack session with advanced anti-detection...");
                var driver = new RemoteWebDriver(
                    new Uri($"https://{_configuration["BrowserStack:Server"]}/wd/hub"),
                    options
                );

                // SCRIPTURI AVANSATE PENTRU A ASCUNDE AUTOMATION-UL
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                // Ascunde webdriver property
                js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");

                // Modifică navigator properties
                js.ExecuteScript(@"
            Object.defineProperty(navigator, 'plugins', {
                get: () => [1, 2, 3, 4, 5],
                configurable: true
            });
            
            Object.defineProperty(navigator, 'languages', {
                get: () => ['en-US', 'en'],
                configurable: true
            });
            
            Object.defineProperty(navigator, 'platform', {
                get: () => 'Win32',
                configurable: true
            });
            
            // Ascunde chrome runtime
            window.chrome = undefined;
        ");

                Console.WriteLine("✅ BrowserStack session started successfully (advanced anti-detection enabled)");
                return driver;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ BrowserStack connection failed: {ex.Message}");
                throw;
            }
        }

        private static IWebDriver CreateBrowserStackDriverWithConfig(string os, string osVersion, string browserName, string browserVersion)
        {
            var browserStackOptions = new Dictionary<string, object>
            {
                ["userName"] = _configuration["BrowserStack:Username"],
                ["accessKey"] = _configuration["BrowserStack:AccessKey"],
                ["os"] = os,
                ["osVersion"] = osVersion,
                ["browserName"] = browserName,
                ["browserVersion"] = browserVersion,
                ["projectName"] = _configuration["BrowserStack:ProjectName"] ?? "Google Tests",
                ["buildName"] = _configuration["BrowserStack:BuildName"] ?? "Cross-Browser Tests",
                ["sessionName"] = $"Google Tests - {browserName} {browserVersion} on {os}",
                ["local"] = _configuration["BrowserStack:Local"] ?? "false",
                ["resolution"] = "1920x1080"
            };

            DriverOptions options = browserName.ToLower() switch
            {
                "firefox" => new FirefoxOptions(),
                "safari" => new SafariOptions(),
                "edge" => new EdgeOptions(),
                _ => new ChromeOptions()
            };

            options.AddAdditionalOption("bstack:options", browserStackOptions);

            return new RemoteWebDriver(
                new Uri($"https://{_configuration["BrowserStack:Server"]}/wd/hub"),
                options.ToCapabilities(),
                TimeSpan.FromSeconds(120)
            );
        }

        private static IWebDriver CreateBrowserStackMobileDriver()
        {
            var browserStackOptions = new Dictionary<string, object>
            {
                ["userName"] = _configuration["BrowserStack:Username"],
                ["accessKey"] = _configuration["BrowserStack:AccessKey"],
                ["deviceName"] = "iPhone 13",
                ["osVersion"] = "15",
                ["browserName"] = "Chrome",
                ["deviceOrientation"] = "portrait",
                ["projectName"] = _configuration["BrowserStack:ProjectName"] ?? "Google Tests",
                ["buildName"] = _configuration["BrowserStack:BuildName"] ?? "Mobile Tests",
                ["sessionName"] = "Google Tests - iPhone 13",
                ["local"] = _configuration["BrowserStack:Local"] ?? "false"
            };

            var options = new ChromeOptions();
            options.AddAdditionalOption("bstack:options", browserStackOptions);

            return new RemoteWebDriver(
                new Uri($"https://{_configuration["BrowserStack:Server"]}/wd/hub"),
                options.ToCapabilities(),
                TimeSpan.FromSeconds(120)
            );
        }

        public static void Close()
        {
            _driver?.Quit();
            _driver = null;
        }
    }
}