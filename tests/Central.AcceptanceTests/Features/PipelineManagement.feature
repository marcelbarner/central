Feature: Pipeline Management
    As a user
    I want to create and manage document processing pipelines
    So that I can orchestrate complex workflows with multiple tasks and wait steps

Background:
    Given the application is running

Scenario: Create a simple pipeline with one task step
    Given a task exists with name "Analyzer"
    When I create a pipeline with name "Simple Analysis"
    And I add a task step using "Analyzer" at order 1
    Then the pipeline should be created successfully
    And the pipeline should have 1 step

Scenario: Create a pipeline with multiple task steps
    Given a task exists with name "Extractor"
    And a task exists with name "Enricher"
    When I create a pipeline with name "Extract and Enrich"
    And I add a task step using "Extractor" at order 1
    And I add a task step using "Enricher" at order 2
    Then the pipeline should be created successfully
    And the pipeline should have 2 steps in the correct order

Scenario: Create a pipeline with wait steps
    Given a task exists with name "Processor"
    When I create a pipeline with name "Delayed Processing"
    And I add a wait step for 5 seconds at order 1
    And I add a task step using "Processor" at order 2
    Then the pipeline should be created successfully
    And the pipeline should have 2 steps

Scenario: Create a pipeline with automatic trigger
    Given a task exists with name "Auto Task"
    When I create a pipeline with name "Auto Pipeline"
    And I set the trigger state to "Imported"
    And I add a task step using "Auto Task" at order 1
    And I enable the pipeline
    Then the pipeline should be created successfully
    And the pipeline should be enabled
    And the trigger state should be "Imported"

Scenario: Create a pipeline for manual execution only
    Given a task exists with name "Manual Task"
    When I create a pipeline with name "Manual Pipeline"
    And I do not set a trigger state
    And I add a task step using "Manual Task" at order 1
    Then the pipeline should be created successfully
    And the trigger state should be null

Scenario: List all pipelines
    Given multiple pipelines exist
    When I request all pipelines
    Then I should see all created pipelines
    And each pipeline should show its steps

Scenario: Update a pipeline
    Given a pipeline exists with 1 step
    And a task exists with name "New Task"
    When I update the pipeline
    And I add a task step using "New Task" at order 2
    Then the pipeline should be updated successfully
    And the pipeline should have 2 steps

Scenario: Delete a pipeline
    Given a pipeline exists
    When I delete the pipeline
    Then the pipeline should be removed
    And it should not appear in the pipeline list

Scenario: Execute a pipeline on a document
    Given a task exists with name "Task 1"
    And a task exists with name "Task 2"
    And a pipeline exists with 2 task steps
    And a document exists
    When I execute the pipeline on the document
    Then a pipeline execution should be created
    And all task steps should execute in order
    And each task should create a task execution
    And all task executions should be linked to the pipeline execution
    And the pipeline execution should complete successfully

Scenario: Pipeline execution with wait step
    Given a task exists with name "Delayed Task"
    And a pipeline exists with a wait step followed by a task step
    And a document exists
    When I execute the pipeline on the document
    Then the pipeline should wait for the specified duration
    And then execute the task step
    And the total execution time should reflect the wait

Scenario: Pipeline execution stops on task failure
    Given a task exists that will fail
    And a task exists with name "Next Task"
    And a pipeline exists with 2 task steps
    And a document exists
    When I execute the pipeline on the document
    Then the first task should fail
    And the pipeline execution should be marked as failed
    And the second task should not execute
    And the error should be captured

Scenario: Automatic pipeline execution via background processing
    Given an enabled pipeline exists for Imported documents
    When a document reaches Imported state
    Then the background worker should detect the document
    And automatically execute the pipeline
    And the document should be processed according to pipeline steps

Scenario: View pipeline execution history
    Given a pipeline has been executed multiple times
    When I retrieve the pipeline execution history
    Then I should see all executions for that pipeline
    And each execution should show document ID and status
    And each execution should show linked task executions
    And executions should be ordered by date

Scenario: Disable a pipeline
    Given an enabled pipeline exists
    When I disable the pipeline
    Then the pipeline should be marked as disabled
    And it should not execute automatically
    And manual execution should still be possible
