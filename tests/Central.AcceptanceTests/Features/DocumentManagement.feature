Feature: Document Management
    As a user
    I want to manage documents
    So that I can store and retrieve important files

Background:
    Given the application is running
    And I am logged in as a test user

Scenario: Upload a document file
    Given I navigate to the documents page
    When I upload a file "test-document.pdf"
    Then the document should be created with title "test-document"
    And the document should appear in the documents list

Scenario: Create a document with full metadata
    Given I navigate to the documents page
    When I create a new document with the following details:
        | Field        | Value                |
        | Title        | Annual Report 2024   |
        | DocumentDate | 2024-12-01           |
        | Content      | Important annual report |
    And I upload the original file "report.pdf"
    Then the document should be created successfully
    And the document details should be visible

Scenario: View document details
    Given I navigate to the documents page
    And a document exists with title "Test Document"
    When I click on the document in the list
    Then I should see the document details page
    And I should see the document title "Test Document"
    And the PDF viewer should display the document

Scenario: Update document metadata
    Given I navigate to the documents page
    And a document exists with title "Original Title"
    When I open the document details
    And I change the title to "Updated Title"
    And I save the changes
    Then the document title should be "Updated Title"
    And the update timestamp should be current

Scenario: Delete a document
    Given I navigate to the documents page
    And a document exists with title "To Be Deleted"
    When I delete the document
    Then the document should no longer appear in the list
    And the associated files should be removed

Scenario: Documents list shows correct information
    Given I navigate to the documents page
    And multiple documents exist
    Then I should see a table with columns "Title", "Document Date", and "Actions"
    And each row should have details and delete buttons

Scenario: Pagination works correctly
    Given I navigate to the documents page
    And more than 10 documents exist
    Then I should see pagination controls
    And only 10 documents should be visible per page
