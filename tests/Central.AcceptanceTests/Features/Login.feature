Feature: User Login
    As a user
    I want to login to the application
    So that I can access the system

Scenario: Successfully login with valid credentials
    Given the application is running
    And I navigate to the login page
    When I enter username "ng-matero" and password "ng-matero"
    And I click the login button
    Then I should be redirected to the home page
    And I should see the user menu

Scenario: Cannot login with invalid credentials
    Given the application is running
    And I navigate to the login page
    When I enter username "invalid" and password "wrong"
    And I click the login button
    Then I should see an error message
    And I should remain on the login page

Scenario: Cannot submit login form with empty fields
    Given the application is running
    And I navigate to the login page
    Then the login button should be disabled
