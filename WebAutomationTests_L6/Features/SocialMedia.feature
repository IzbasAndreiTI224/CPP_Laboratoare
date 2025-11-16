Feature: Social Media Links Verification
    As a user
    I want to verify that the social media links in footer work correctly
    So that I can follow the website on social platforms

@socialmedia
Scenario: Verify social media links functionality
    Given the user is on the home page
    When the user scrolls to the footer
    Then the social media icons should be visible
    When the user clicks on each social media icon
    Then each link should open in a new tab
    And the URLs should correspond to correct social media pages