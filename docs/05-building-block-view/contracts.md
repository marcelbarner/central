# Contract Management

## Overview

The contract management module allows users to create and manage contracts that can be associated with documents. A contract represents a legal or business agreement and can be linked to multiple documents while each document can only be associated with one contract.

## Domain Model

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Component.puml

class Contract {
  + Id: long
  + Name: string
  + Description: string?
  + State: ContractState
  + CorrespondentId: long?
  + Created: DateTimeOffset
  + Updated: DateTimeOffset
}

enum ContractState {
  DRAFT
  ACTIVE
  EXPIRED
  TERMINATED
}

class Document {
  + Id: long
  + Title: string
  + ContractId: long?
  ...
}

class Correspondent {
  + Id: long
  + Name: string
  ...
}

Contract "1" -- "0..1" Correspondent : associated with >
Contract "1" -- "0..*" Document : contains >
Document "0..*" -- "0..1" Contract : belongs to >

@enduml
```

## Key Features

### Contract Properties

- **Name**: Required, unique identifier for the contract (max 200 characters)
- **Description**: Optional detailed description (max 1000 characters)
- **State**: Contract lifecycle state (Draft, Active, Expired, Terminated)
- **Correspondent**: Optional reference to the counterparty of the contract

### Business Rules

1. **One-to-Many Relationship**: A contract can be linked to multiple documents, but each document can only be associated with one contract at a time.

2. **Correspondent Synchronization**: When assigning a contract to a document, the system prompts the user whether to update the document's correspondent to match the contract's correspondent.

3. **State Management**: Contracts can transition through states:
   - **Draft**: Initial state for newly created contracts
   - **Active**: Contracts that are currently in effect
   - **Expired**: Contracts that have reached their end date
   - **Terminated**: Contracts that were ended prematurely

## Repository Ports

The domain defines the following port for contract persistence:

```csharp
public interface IContractRepository
{
    Task<Contract> AddAsync(Contract contract, CancellationToken cancellationToken);
    Task<Contract> UpdateAsync(Contract contract, CancellationToken cancellationToken);
    Task<Contract?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Contract?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Contract>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken);
}
```

## Domain Services

### IContractService

The contract service provides business logic for contract operations:

- **CreateAsync**: Creates a new contract with validation
- **UpdateAsync**: Updates contract properties and state
- **GetByIdAsync**: Retrieves a contract by ID
- **GetAllAsync**: Retrieves all contracts
- **DeleteAsync**: Deletes a contract (only if no documents are associated)
- **AssignToDocumentAsync**: Assigns a contract to a document with optional correspondent update

## Integration Points

### Document Service Extension

When assigning a contract to a document:
1. Validate that the contract exists
2. Check if the document already has a different contract assigned
3. Optionally update the document's correspondent to match the contract's correspondent
4. Update the document with the new contract reference

### API Endpoints

- `GET /api/contracts` - List all contracts
- `GET /api/contracts/{id}` - Get contract details
- `POST /api/contracts` - Create new contract
- `PUT /api/contracts/{id}` - Update contract
- `DELETE /api/contracts/{id}` - Delete contract
- `POST /api/contracts/{id}/assign-to-document` - Assign contract to document

## UI Components

### Contract List Page
- Displays all contracts in a table
- Shows contract name, state, correspondent, and document count
- Provides actions for viewing, editing, and deleting contracts
- Includes button to create new contract

### Contract Details Page
- Shows complete contract information
- Lists all documents associated with the contract
- Allows editing of contract properties
- Shows contract state transitions

### Contract Creation Dialog
- Modal dialog for creating new contracts
- Form fields for name, description, state, and correspondent
- Validation for required fields and length limits
- Integration with correspondent selection

### Document Assignment UI
- Dropdown/autocomplete for selecting a contract on document details
- Confirmation dialog asking whether to sync correspondent from contract
- Visual indicator showing current contract assignment
