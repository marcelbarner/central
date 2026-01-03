Feature: Document Processing
    As a user
    I want to configure and execute automated document processing workflows
    So that documents can be automatically analyzed and enriched

Background:
    Given the application is running

Scenario: Create a process definition with steps
    When I create a process definition with name "Document Import Process"
    And I add an Azure Document Intelligence step to extract content
    And I add an Azure OpenAI step to enrich metadata
    Then the process definition should be created successfully
    And the process should have 2 steps in the correct order

Scenario: Execute a process on an imported document
    Given a process definition exists for documents in Imported state
    And a document exists with state Imported
    When I trigger the process execution for the document
    Then the process execution should be created
    And the document state should be Processing
    And each step should execute in order
    And the document state should be Processed when complete

Scenario: View process execution history for a document
    Given a document has multiple completed process executions
    When I retrieve the execution history for the document
    Then I should see all executions ordered by date
    And each execution should show step results
    And execution status should be visible

Scenario: Automatic background processing
    Given an enabled process definition exists for Imported documents
    When a document is uploaded and reaches Imported state
    Then the background worker should automatically detect it
    And create a process execution within 30 seconds
    And execute all configured steps

Scenario: Handle step failure gracefully
    Given a process with a step that will fail
    When the process executes on a document
    Then the execution should stop at the failed step
    And the document state should be Failed
    And the error message should be captured
    And subsequent steps should not execute
