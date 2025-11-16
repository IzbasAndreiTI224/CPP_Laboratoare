namespace GoogleTests.Support
{
    public static class Locators
    {
        // Search page elements - updated for current Google structure
        public static string SearchInput => "//textarea[@name='q']";
        public static string SearchButton => "//input[@name='btnK'] | //input[@value='Google Search'] | //button[@type='submit']";
        public static string SearchResults => "//div[@id='search']//div[contains(@class, 'g ')] | //div[@id='rso']//div[contains(@class, 'g ')] | //div[contains(@class, 'g ')]";
        public static string DidYouMean => "//*[contains(., 'Did you mean') or contains(., 'Ai vrut să spui') or contains(., 'Showing results for') or contains(., 'Caută mai degrabă') or contains(., 'Search instead') or contains(., 'Искать вместо этого') or contains(., 'spell_orig')]";
        public static string GoogleLogo => "//img[@alt='Google'] | //div[@id='hplogo'] | //img[@alt='Google' and @class='lnXdpd']";
        public static string ImFeelingLucky => "//input[@name='btnI']";

        // Generic elements
        public static string PageTitle => "//title";
        public static string Body => "//body";
        public static string AcceptCookies => "//button[contains(., 'Accept all') or contains(., 'Acceptă tot')]";
    }
}