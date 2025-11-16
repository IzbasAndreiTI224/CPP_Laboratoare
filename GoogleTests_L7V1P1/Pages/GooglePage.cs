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
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); // Redus de la 30 la 10 secunde
        }

        public void NavigateToGoogle()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _driver.Navigate().GoToUrl(configuration["BaseUrl"]);
            AcceptCookiesIfPresent();
            WaitForPageToLoad();
        }

        public void EnterSearchTerm(string searchTerm)
        {
            var searchInput = _wait.Until(d => d.FindElement(By.XPath(Locators.SearchInput)));
            searchInput.Clear();
            searchInput.SendKeys(searchTerm);
        }

        public void ClickSearch()
        {
            try
            {
                // Folosește Enter în loc de butonul de search
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
                Thread.Sleep(2000); // Așteaptă 2 secunde pentru rezultate
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
                var element = _driver.FindElement(By.XPath(Locators.DidYouMean));
                return element.Displayed;
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
                // Așteaptă puțin pentru butonul de cookies
                Thread.Sleep(1000);
                var acceptButton = _driver.FindElements(By.XPath(Locators.AcceptCookies))
                                         .FirstOrDefault(e => e.Displayed);
                if (acceptButton != null)
                {
                    acceptButton.Click();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception)
            {
                // Ignoră erorile la cookies
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