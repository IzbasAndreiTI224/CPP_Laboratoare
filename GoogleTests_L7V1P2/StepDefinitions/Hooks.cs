using TechTalk.SpecFlow;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using GoogleTests.Support;

namespace GoogleTests.Step_definitions
{
    [Binding]
    public class Hooks
    {
        private static AventStack.ExtentReports.ExtentReports _extent;
        private static AventStack.ExtentReports.ExtentTest _feature;
        private AventStack.ExtentReports.ExtentTest _scenario;
        private readonly ScenarioContext _scenarioContext;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Driver.Initialize();

            // Calea pentru folderul principal al proiectului
            var baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var reportsPath = Path.Combine(baseDirectory, "Reports");
            Directory.CreateDirectory(reportsPath);

            // Creează raportul direct în folderul Reports, nu într-un subfolder
            var htmlReporter = new ExtentHtmlReporter(Path.Combine(reportsPath, "index.html"));
            _extent = new AventStack.ExtentReports.ExtentReports();
            _extent.AttachReporter(htmlReporter);
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            _feature = _extent.CreateTest(featureContext.FeatureInfo.Title);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _scenario = _feature.CreateNode(_scenarioContext.ScenarioInfo.Title);
        }

        [AfterStep]
        public void AfterStep()
        {
            var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepName = _scenarioContext.StepContext.StepInfo.Text;

            if (_scenarioContext.TestError == null)
            {
                _scenario.Pass($"{stepType}: {stepName}");
            }
            else
            {
                _scenario.Fail($"{stepType}: {stepName} - Error: {_scenarioContext.TestError.Message}");

                var screenshot = ((ITakesScreenshot)Driver.Current).GetScreenshot();
                var baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                var reportsPath = Path.Combine(baseDirectory, "Reports");
                var screenshotPath = Path.Combine(reportsPath, $"screenshot_{DateTime.Now:yyyyMMddHHmmss}.png");
                screenshot.SaveAsFile(screenshotPath);
                _scenario.AddScreenCaptureFromPath(screenshotPath);
            }
        }

        [AfterScenario]
        public void AfterScenario()
        {
            // Clean up
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _extent.Flush();
            Driver.Close();
        }
    }
}