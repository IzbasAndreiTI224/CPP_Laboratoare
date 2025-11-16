Feature: Quick View Button Verification
    As a user
    I want to verify that the Quick View button works correctly
    So that I can view product details without issues

@quickview
Scenario: Verify Quick View button displays correct product details
    Given the user is on the home page
    When the user clicks on a product's Quick View button
    Then the product information should be displayed
    And the displayed details should match the selected product
    When the user navigates back to home page
    Then the user should be redirected back to the main page