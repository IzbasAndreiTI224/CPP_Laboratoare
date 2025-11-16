Feature: Google Search Functionality
  As a user
  I want to use Google search
  So that I can find information on the web

  Scenario: Google page opens successfully
    Given I navigate to Google
    Then the Google page should be loaded successfully

  Scenario: Search returns multiple results
    Given I am on the Google search page
    When I search for "test automation"
    Then I should see multiple search results

  Scenario: Empty search does nothing
    Given I am on the Google search page
    When I perform an empty search
    Then I should remain on the search page

  Scenario: Search shows "Did you mean" for irrelevant terms
    Given I am on the Google search page
    When I search for an irrelevant term "asdfghjklqwerty"
    Then I should see the "Did you mean" suggestion