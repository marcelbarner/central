Feature: Say Hello
    As a user
    I want to send a greeting request
    So that I can receive a personalized hello message

Scenario: Successfully greet a user with valid names
    Given the application is running
    When I send a greeting with first name "John" and last name "Smith"
    Then the response should be successful
    And the greeting message should be "Hello John Smith..."

Scenario: Reject greeting with too short first name
    Given the application is running
    When I send a greeting with first name "Jo" and last name "Smith"
    Then the response should indicate validation error

Scenario: Reject greeting with too short last name
    Given the application is running
    When I send a greeting with first name "John" and last name "Doe"
    Then the response should indicate validation error
