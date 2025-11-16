Feature: Home Page Verification
    As a user
    I want to verify that the home page loads correctly
    So that I can ensure the website is working properly

@homepage
Scenario: Verify home page loads successfully
    Given the browser is opened
    When the user navigates to "https://adoring-pasteur-3ae17d.netlify.app/"
    And the user waits for the page to load completely
    Then the logo should be displayed
    And the main menu should be displayed
    And images should be present on the page
    And no critical console errors should be present
    And the page load time should be reasonable