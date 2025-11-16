public static class Locators
{
    // Search page elements
    public static string SearchInput => "//textarea[@name='q']";
    public static string SearchButton => "//input[@name='btnK'] | //input[@value='Google Search'] | //button[@type='submit']";
    public static string SearchResults => "//div[@id='search']//div[@class='g'] | //div[@id='rso']//div[contains(@class, 'g ')]";
    public static string DidYouMean => "//*[contains(., 'Did you mean') or contains(., 'Ai vrut să spui') or contains(., 'Search instead') or contains(., 'Искать вместо этого') or contains(., 'spell_orig')]";
    public static string GoogleLogo => "//img[@alt='Google'] | //div[@id='hplogo'] | //img[@alt='Google' and @class='lnXdpd']";

    // Cookie consent in multiple languages
    public static string AcceptCookies => "//button[contains(., 'Accept all') or contains(., 'Alles accepteren') or contains(., 'Acceptă tot') or contains(., 'I accept') or contains(., 'Aceptar todo') or contains(., 'Tout accepter')]";
    public static string RejectCookies => "//button[contains(., 'Reject all') or contains(., 'Alles afwijzen') or contains(., 'Respinge tot')]";

    // Generic elements
    public static string PageTitle => "//title";
    public static string Body => "//body";
}