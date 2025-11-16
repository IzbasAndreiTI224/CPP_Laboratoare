using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

class Program
{
    static void Main()
    {
        // Configurează driver-ul Chrome
        IWebDriver driver = new ChromeDriver();

        try
        {
            // Deschide 9gag.com
            driver.Navigate().GoToUrl("https://9gag.com");
            Console.WriteLine("Am deschis 9gag.com");

            // Așteaptă ca pagina să se încarce complet
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Verifică dacă antetul 9gag este afișat
            bool isHeaderDisplayed = CheckIfHeaderIsDisplayed(driver);
            Console.WriteLine($"Antetul 9gag este afisat: {isHeaderDisplayed}");

            // Caută "computer" folosind butonul de search
            SearchForComputer(driver);

            // Verifică din nou antetul după căutare
            isHeaderDisplayed = CheckIfHeaderIsDisplayed(driver);
            Console.WriteLine($"După cautare, antetul 9gag este afisat: {isHeaderDisplayed}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"A aparut o eroare: {ex.Message}");
        }
        finally
        {
            // Opțional: păstrează browser-ul deschis pentru a vedea rezultatul
            Console.WriteLine("Apasa orice tasta pentru a inchide browser-ul...");
            Console.ReadKey();

            // Închide browser-ul
            driver.Quit();
        }
    }

    static bool CheckIfHeaderIsDisplayed(IWebDriver driver)
    {
        try
        {
            // Încearcă să găsești antetul folosind diferite selectori comune
            var headerSelectors = new[]
            {
                "header",
                ".header",
                "#header",
                "nav",
                ".navbar",
                "[data-test='header']"
            };

            foreach (var selector in headerSelectors)
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector(selector));
                    if (element.Displayed)
                    {
                        Console.WriteLine($"Am gasit antetul folosind selectorul: {selector}");
                        return true;
                    }
                }
                catch (NoSuchElementException)
                {
                    // Continuă cu următorul selector
                    continue;
                }
            }

            Console.WriteLine("Nu am putut gasi antetul cu niciun selector");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la verificarea antetului: {ex.Message}");
            return false;
        }
    }

    static void SearchForComputer(IWebDriver driver)
    {
        try
        {
            // Găsește butonul de search după clasa "search"
            var searchButton = driver.FindElement(By.CssSelector("a.search"));

            if (searchButton.Displayed && searchButton.Enabled)
            {
                Console.WriteLine("Am gasit butonul de search");

                // Click pe butonul de search pentru a deschide bara de căutare
                searchButton.Click();
                Console.WriteLine("Am apasat pe butonul de search");

                // Așteaptă puțin pentru ca bara de căutare să apară
                System.Threading.Thread.Sleep(2000);

                // Acum căutăm input-ul de căutare folosind selectorul exact
                var searchInput = driver.FindElement(By.CssSelector("div.ui-input input[type='text']"));

                if (searchInput != null && searchInput.Displayed)
                {
                    // Introdu "computer" în bara de căutare
                    searchInput.Clear();
                    searchInput.SendKeys("computer");
                    Console.WriteLine("Am introdus 'computer' in bara de cautare");

                    // Apasă Enter pentru a efectua căutarea
                    searchInput.SendKeys(Keys.Enter);
                    Console.WriteLine("Am apasat Enter pentru a cauta");

                    // Așteaptă ca rezultatele să se încarce
                    System.Threading.Thread.Sleep(3000);
                    Console.WriteLine("Am asteptat incarcarea rezultatelor");
                }
                else
                {
                    Console.WriteLine("Input-ul de cautare nu este vizibil");
                }
            }
            else
            {
                Console.WriteLine("Butonul de search nu este vizibil sau activ");
            }
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine($"Elementul nu a putut fi gasit: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la cautare: {ex.Message}");
        }
    }
}