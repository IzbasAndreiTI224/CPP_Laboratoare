Feature: Navigation Menu Verification
    As a user
    I want to verify that the navigation menu works correctly
    So that I can navigate through the website properly

@navigation
Scenario: Verify navigation menu functionality
    Given the user is on the home page
    Then the main menu should be visible in the top part of the page
    When the user selects each menu option
    Then the page URL should change accordingly
    And the page content should update for each selection
    When the user returns to home page by selecting the logo
    Then the user should be redirected back to the main page
    When the user repeats the navigation actions
    Then the results should be consistent each time