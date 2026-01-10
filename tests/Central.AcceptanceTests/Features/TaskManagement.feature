Feature: Task Management
    As a user
    I want to create and manage reusable AI processing tasks
    So that I can execute them individually or as part of pipelines

Background:
    Given the application is running

Scenario: Create an Azure OpenAI task
    When I create a task with name "Metadata Extractor"
    And I set the task type to "AzureOpenAI"
    And I configure the Azure endpoint "https://test.openai.azure.com"
    And I set the model to "gpt-4"
    And I set the prompt to "Extract metadata from this document"
    Then the task should be created successfully
    And the task should be enabled by default

Scenario: Create an Azure Document Intelligence task
    When I create a task with name "Form Analyzer"
    And I set the task type to "AzureDocumentIntelligence"
    And I configure the Azure endpoint "https://test.cognitiveservices.azure.com"
    And I set the model to "prebuilt-invoice"
    Then the task should be created successfully

Scenario: List all tasks
    Given multiple tasks exist
    When I request all tasks
    Then I should see all created tasks
    And each task should show its configuration

Scenario: Update a task
    Given a task exists with name "Old Name"
    When I update the task name to "New Name"
    And I update the prompt
    Then the task should be updated successfully
    And the changes should be persisted

Scenario: Delete a task
    Given a task exists
    When I delete the task
    Then the task should be removed
    And it should not appear in the task list

Scenario: Execute a task directly on a document
    Given a task exists with name "Content Analyzer"
    And a document exists
    When I execute the task on the document
    Then a task execution should be created
    And the execution should complete successfully
    And the execution result should be available
    And the execution should not be linked to a pipeline

Scenario: View task execution history
    Given a task has been executed multiple times
    When I retrieve the task execution history
    Then I should see all executions for that task
    And each execution should show document ID and status
    And executions should be ordered by date

Scenario: Disable a task
    Given an enabled task exists
    When I disable the task
    Then the task should be marked as disabled
    And it should not execute automatically
