Feature: Contract Management
    As a user
    I want to manage contracts
    So that I can associate documents with contracts and track their relationships

Background:
    Given the application is running

Scenario: Create a new contract
    When I create a contract with the following details:
        | Name              | Test Contract           |
        | Description       | A test contract         |
        | State             | Draft                   |
    Then the contract should be created successfully
    And the contract should have name "Test Contract"
    And the contract should have state "Draft"

Scenario: Create a contract with a correspondent
    Given a correspondent exists with name "ACME Corp"
    When I create a contract with the following details:
        | Name              | ACME Contract           |
        | Description       | Contract with ACME      |
        | State             | Active                  |
        | CorrespondentName | ACME Corp               |
    Then the contract should be created successfully
    And the contract should have correspondent "ACME Corp"

Scenario: Get all contracts
    Given the following contracts exist:
        | Name         | State  |
        | Contract A   | Draft  |
        | Contract B   | Active |
    When I retrieve all contracts
    Then I should see 2 contracts
    And the contracts should include "Contract A"
    And the contracts should include "Contract B"

Scenario: Get contract by ID
    Given a contract exists with name "Existing Contract"
    When I retrieve the contract by its ID
    Then the contract details should be returned
    And the contract should have name "Existing Contract"

Scenario: Update contract details
    Given a contract exists with name "Old Name"
    When I update the contract with the following details:
        | Name        | New Name            |
        | Description | Updated description |
        | State       | Active              |
    Then the contract should be updated successfully
    And the contract should have name "New Name"
    And the contract should have state "Active"

Scenario: Delete a contract without documents
    Given a contract exists with name "Contract To Delete"
    When I delete the contract
    Then the contract should be deleted successfully

Scenario: Cannot delete a contract with associated documents
    Given a contract exists with name "Contract With Docs"
    And a document exists with title "Test Document"
    And the document is assigned to the contract "Contract With Docs"
    When I delete the contract
    Then the deletion should fail
    And an error message should indicate documents are still associated

Scenario: Assign contract to document
    Given a contract exists with name "Service Agreement"
    And a document exists with title "Invoice 001"
    When I assign the contract "Service Agreement" to the document "Invoice 001"
    Then the document should be associated with the contract
    And the document should have contract "Service Agreement"

Scenario: Assign contract to document with correspondent sync
    Given a correspondent exists with name "Company XYZ"
    And a contract exists with name "XYZ Contract" and correspondent "Company XYZ"
    And a document exists with title "Document ABC" without correspondent
    When I assign the contract "XYZ Contract" to the document "Document ABC" with correspondent sync enabled
    Then the document should be associated with the contract
    And the document should have correspondent "Company XYZ"

Scenario: Assign contract to document without correspondent sync
    Given a correspondent exists with name "Company ABC"
    And a correspondent exists with name "Company XYZ"
    And a contract exists with name "XYZ Contract" and correspondent "Company XYZ"
    And a document exists with title "Document ABC" with correspondent "Company ABC"
    When I assign the contract "XYZ Contract" to the document "Document ABC" with correspondent sync disabled
    Then the document should be associated with the contract
    And the document should still have correspondent "Company ABC"

Scenario: Change document contract assignment
    Given a contract exists with name "Old Contract"
    And a contract exists with name "New Contract"
    And a document exists with title "Movable Document"
    And the document is assigned to the contract "Old Contract"
    When I assign the contract "New Contract" to the document "Movable Document"
    Then the document should be associated with the contract "New Contract"
    And the document should not be associated with the contract "Old Contract"
