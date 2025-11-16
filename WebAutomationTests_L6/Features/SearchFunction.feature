Feature: Search Function Verification
    As a user
    I want to verify that the search function works correctly
    So that I can find products easily

@search
Scenario: Verify search function returns relevant results
    Given the user is on the home page
    When the user searches for "Shirt"
    Then relevant products containing "Shirt" should be displayed
    When the user searches for invalid term "xyz123"
    Then the search term should be accepted
    And the system should display "Not Found" message or empty results

@search @bug
Scenario: Verify search function bug LB-3 - returns 404 error
    Given the user is on the home page
    When the user searches for "Shirt"
    Then the system should not display 404 error page
    But currently the search shows 404 error page confirming bug LB-3