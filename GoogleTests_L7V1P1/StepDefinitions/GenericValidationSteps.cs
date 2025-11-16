using GoogleTests.Pages;
using GoogleTests.Support;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace GoogleTests.Step_definitions
{
    [Binding]
    public class GenericValidationSteps
    {
        private readonly GooglePage _googlePage;

        public GenericValidationSteps()
        {
            _googlePage = new GooglePage(Driver.Current);
        }

        [Then(@"the Google page should be loaded successfully")]
        public void ThenTheGooglePageShouldBeLoadedSuccessfully()
        {
            // Verifică dacă pagina s-a încărcat corect
            var pageTitle = _googlePage.GetPageTitle();
            Assert.That(pageTitle, Is.Not.Empty, "Page title is empty");

            // Verifică dacă input-ul de căutare este prezent (alternativă la logo)
            try
            {
                var searchInput = Driver.Current.FindElement(By.XPath(Locators.SearchInput));
                Assert.That(searchInput.Displayed, Is.True, "Search input is not displayed");
            }
            catch (NoSuchElementException)
            {
                // Dacă nici input-ul nu este găsit, atunci eșuează
                Assert.Fail("Google page not loaded properly - search input not found");
            }
        }

        [Then(@"I should remain on the search page")]
        public void ThenIShouldRemainOnTheSearchPage()
        {
            // Verifică dacă input-ul de căutare este încă prezent (mai robust decât logo-ul)
            var isSearchInputDisplayed = _googlePage.IsSearchInputDisplayed();
            Assert.That(isSearchInputDisplayed, Is.True, "Not on search page - search input not displayed");

            var currentUrl = _googlePage.GetCurrentUrl();
            Assert.That(currentUrl, Does.Contain("google.com"), $"Not on Google page. Current URL: {currentUrl}");
        }

        [Then(@"the page should contain element with locator ""(.*)""")]
        public void ThenThePageShouldContainElementWithLocator(string locator)
        {
            var element = Driver.Current.FindElement(By.XPath(locator));
            Assert.That(element.Displayed, Is.True, $"Element with locator {locator} is not displayed");
        }

        [Then(@"the page title should contain ""(.*)""")]
        public void ThenThePageTitleShouldContain(string expectedText)
        {
            var actualTitle = Driver.Current.Title;
            Assert.That(actualTitle, Does.Contain(expectedText),
                $"Page title '{actualTitle}' does not contain '{expectedText}'");
        }
    }
}