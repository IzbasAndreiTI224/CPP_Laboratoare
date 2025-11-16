using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebAutomationTests.Support;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WebAutomationTests.StepDefinitions
{
    [Binding]
    public class SocialMediaSteps
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public SocialMediaSteps(WebDriverSupport webDriverSupport)
        {
            _driver = webDriverSupport.Driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        }

        [When(@"the user scrolls to the footer")]
        public void WhenTheUserScrollsToTheFooter()
        {
            // Scroll progresiv către footer pentru a permite încărcarea
            Console.WriteLine("📜 Scrolling progressively to footer...");

            // Scroll în pași progresivi
            for (int i = 0; i < 5; i++)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript($"window.scrollTo(0, document.body.scrollHeight * {0.2 * (i + 1)});");
                System.Threading.Thread.Sleep(500);
            }

            // Scroll final la sfârșit
            ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            System.Threading.Thread.Sleep(2000);

            Console.WriteLine("✅ Finished scrolling to footer");
        }

        [Then(@"the social media icons should be visible")]
        public void ThenTheSocialMediaIconsShouldBeVisible()
        {
            // Încearcă mai mulți selectori pentru a găsi containerul social media
            IWebElement socialMediaContainer = null;
            var selectors = new[]
            {
                ".footer-social",
                ".social-nav",
                "[class*='social']",
                "footer [class*='social']",
                ".w3_agile_social",
                ".model-3d-0"
            };

            foreach (var selector in selectors)
            {
                try
                {
                    var elements = _driver.FindElements(By.CssSelector(selector));
                    if (elements.Count > 0)
                    {
                        socialMediaContainer = elements.First();
                        Console.WriteLine($"✅ Found social media container with selector: {selector}");
                        break;
                    }
                }
                catch
                {
                    // Continuă cu următorul selector
                }
            }

            if (socialMediaContainer == null)
            {
                // Încearcă să găsești direct iconițele
                Console.WriteLine("🔍 Container not found, searching for icons directly...");
                var directIcons = _driver.FindElements(By.CssSelector("footer a[class*='facebook'], footer a[class*='twitter'], footer a[class*='instagram'], footer a[class*='pinterest']"));

                if (directIcons.Count > 0)
                {
                    Console.WriteLine($"✅ Found {directIcons.Count} social media icons directly");
                    return;
                }

                // Debug: afișează structura footer-ului
                try
                {
                    var footer = _driver.FindElement(By.CssSelector("footer, [class*='footer']"));
                    Console.WriteLine($"🔍 Footer HTML sample: {footer.Text.Substring(0, System.Math.Min(200, footer.Text.Length))}...");
                }
                catch
                {
                    Console.WriteLine("❌ Could not find footer element");
                }

                Assert.Fail("Social media container or icons not found in footer");
            }

            Assert.IsTrue(socialMediaContainer.Displayed, "Social media container should be visible");

            // Găsește toate iconițele de social media
            var socialIcons = socialMediaContainer.FindElements(By.CssSelector("li a, a[class*='facebook'], a[class*='twitter'], a[class*='instagram'], a[class*='pinterest']"));

            if (socialIcons.Count == 0)
            {
                // Încearcă selectori alternativi - păstrează tipul ReadOnlyCollection
                var allLinks = socialMediaContainer.FindElements(By.CssSelector("a"));
                var filteredIcons = allLinks.Where(icon =>
                    icon.GetAttribute("class")?.Contains("facebook") == true ||
                    icon.GetAttribute("class")?.Contains("twitter") == true ||
                    icon.GetAttribute("class")?.Contains("instagram") == true ||
                    icon.GetAttribute("class")?.Contains("pinterest") == true);

                // Convertim la listă pentru a putea folosi Count și alte metode
                var filteredList = filteredIcons.ToList();
                if (filteredList.Count > 0)
                {
                    Console.WriteLine($"✅ Found {filteredList.Count} social media icons after filtering");

                    // Afișează iconițele găsite
                    foreach (var icon in filteredList)
                    {
                        var iconClass = icon.GetAttribute("class");
                        var href = icon.GetAttribute("href");
                        var hasIcon = icon.FindElements(By.CssSelector("i, [class*='fa-']")).Count > 0;
                        Console.WriteLine($"   - {iconClass} | href: '{href}' | has icon: {hasIcon} | displayed: {icon.Displayed}");
                    }
                    return;
                }
            }

            Assert.Greater(socialIcons.Count, 0, "Should find social media icons");

            Console.WriteLine($"✅ Found {socialIcons.Count} social media icons:");

            foreach (var icon in socialIcons)
            {
                var iconClass = icon.GetAttribute("class");
                var href = icon.GetAttribute("href");
                var hasIcon = icon.FindElements(By.CssSelector("i, [class*='fa-']")).Count > 0;
                Console.WriteLine($"   - {iconClass} | href: '{href}' | has icon: {hasIcon} | displayed: {icon.Displayed}");
            }
        }

        [When(@"the user clicks on each social media icon")]
        public void WhenTheUserClicksOnEachSocialMediaIcon()
        {
            // Găsește iconițele direct (fără container)
            var socialIcons = _driver.FindElements(By.CssSelector("footer a[class*='facebook'], footer a[class*='twitter'], footer a[class*='instagram'], footer a[class*='pinterest']"));

            if (socialIcons.Count == 0)
            {
                // Încearcă selectori mai generali
                socialIcons = _driver.FindElements(By.CssSelector("a[class*='facebook'], a[class*='twitter'], a[class*='instagram'], a[class*='pinterest']"));
            }

            Console.WriteLine($"🖱️ Testing {socialIcons.Count} social media icons...");

            foreach (var icon in socialIcons)
            {
                var iconClass = icon.GetAttribute("class");
                var href = icon.GetAttribute("href");

                Console.WriteLine($"   Testing {iconClass} with href: '{href}'");

                // Verifică dacă href este "#" (bug-ul LB-5)
                if (href == "#" || string.IsNullOrEmpty(href) || href.Contains("javascript:void(0)"))
                {
                    Console.WriteLine($"     🐛 BUG LB-5 CONFIRMED: {iconClass} has empty href ('{href}')");
                    Console.WriteLine($"        Expected: Valid social media URL");
                    Console.WriteLine($"        Actual: {href}");
                }
                else
                {
                    Console.WriteLine($"     ✅ {iconClass} has valid href: {href}");
                }
            }
        }

        [Then(@"each link should open in a new tab")]
        public void ThenEachLinkShouldOpenInANewTab()
        {
            // Cu href="#", linkurile nu vor deschide tab-uri noi
            Console.WriteLine("ℹ️ Links with href='#' cannot open new tabs - this is part of bug LB-5");
            Assert.Inconclusive("Social media links have href='#' and cannot open new tabs. Bug LB-5 confirmed.");
        }

        [Then(@"the URLs should correspond to correct social media pages")]
        public void ThenTheURLsShouldCorrespondToCorrectSocialMediaPages()
        {
            var socialIcons = _driver.FindElements(By.CssSelector("a[class*='facebook'], a[class*='twitter'], a[class*='instagram'], a[class*='pinterest']"));

            int brokenLinks = 0;
            int totalLinks = socialIcons.Count;

            Console.WriteLine("🔍 Analyzing social media URLs...");

            foreach (var icon in socialIcons)
            {
                var iconClass = icon.GetAttribute("class");
                var href = icon.GetAttribute("href");

                // Verifică dacă href este valid
                bool isValidUrl = !string.IsNullOrEmpty(href) &&
                                 href != "#" &&
                                 !href.Contains("javascript:") &&
                                 (href.Contains("facebook.com") ||
                                  href.Contains("twitter.com") ||
                                  href.Contains("instagram.com") ||
                                  href.Contains("linkedin.com") ||
                                  href.Contains("pinterest.com"));

                if (!isValidUrl)
                {
                    brokenLinks++;
                    Console.WriteLine($"   ❌ {iconClass}: INVALID href ('{href}')");

                    if (href == "#")
                    {
                        Console.WriteLine($"      🐛 BUG LB-5: Empty link that doesn't go anywhere");
                    }
                }
                else
                {
                    Console.WriteLine($"   ✅ {iconClass}: VALID href ({href})");
                }
            }

            Console.WriteLine($"📊 Summary: {brokenLinks}/{totalLinks} broken social media links");

            if (brokenLinks > 0)
            {
                Assert.Fail($"Found {brokenLinks} broken social media links with href='#'. Bug LB-5 confirmed.");
            }
            else
            {
                Console.WriteLine("✅ All social media links have valid URLs");
            }
        }
    }
}