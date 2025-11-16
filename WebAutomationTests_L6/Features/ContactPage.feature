Feature: Contact Page Verification
    As a user
    I want to verify that the Contact page works correctly
    So that I can contact the website owners when needed

@contact
Scenario: Verify Contact page functionality
    Given the user is on the home page
    When the user navigates to the Contact page
    Then the URL should change to "/contact"
    And the contact form should be visible with required fields
    When the user returns to home page by selecting the logo
    Then the home page should be displayed correctly