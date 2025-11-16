using GoogleTests.Pages;
using GoogleTests.Support;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace GoogleTests.Step_definitions
{
    [Binding]
    public class GoogleSearchSteps
    {
        private readonly GooglePage _googlePage;
        private readonly ScenarioContext _scenarioContext;

        public GoogleSearchSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _googlePage = new GooglePage(Driver.Current);
        }

        [Given(@"I navigate to Google")]
        public void GivenINavigateToGoogle()
        {
            _googlePage.NavigateToGoogle();
        }

        [Given(@"I am on the Google search page")]
        public void GivenIAmOnTheGoogleSearchPage()
        {
            _googlePage.NavigateToGoogle();
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string searchTerm)
        {
            _googlePage.EnterSearchTerm(searchTerm);
            _googlePage.PressEnterInSearch();
        }

        [When(@"I perform an empty search")]
        public void WhenIPerformAnEmptySearch()
        {
            _googlePage.ClickSearch();
        }

        [When(@"I search for an irrelevant term ""(.*)""")]
        public void WhenISearchForAnIrrelevantTerm(string irrelevantTerm)
        {
            _googlePage.EnterSearchTerm(irrelevantTerm);
            _googlePage.PressEnterInSearch();
            // Așteaptă mai mult pentru sugestia "Did you mean"
            Thread.Sleep(3000);
        }

        [Then(@"I should see multiple search results")]
        public void ThenIShouldSeeMultipleSearchResults()
        {
            // Așteaptă mai mult pentru rezultate
            Thread.Sleep(3000);
            var resultsCount = _googlePage.GetSearchResultsCount();
            Assert.That(resultsCount, Is.GreaterThan(0), $"No search results displayed. Found {resultsCount} results");
        }

        [Then(@"I should see the ""Did you mean"" suggestion")]
        public void ThenIShouldSeeTheDidYouMeanSuggestion()
        {
            // Așteaptă mai mult pentru sugestie
            Thread.Sleep(3000);
            var isDisplayed = _googlePage.IsDidYouMeanDisplayed();
            Assert.That(isDisplayed, Is.True, "Did you mean suggestion was not displayed");
        }
    }
}