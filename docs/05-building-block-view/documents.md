# Document Management

## Overview

The document management module allows users to store, manage, and view documents with associated metadata and file attachments.

## Domain Model

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Component.puml

class Document {
  + Id: long
  + Title: string
  + DocumentDate: DateTimeOffset?
  + Content: string?
  + OriginalFile: OriginalFile?
  + ArchiveFile: ArchiveFile?
  + Thumbnail: Thumbnail?
  + Added: DateTimeOffset
  + Updated: DateTimeOffset
  + AddedBy: string
  + UpdatedBy: string
}

class OriginalFile {
  + FileName: string
  + FilePath: string
}

class ArchiveFile {
  + FileName: string
  + FilePath: string
}

class Thumbnail {
  + FileName: string
  + FilePath: string
}

Document "1" *-- "0..1" OriginalFile
Document "1" *-- "0..1" ArchiveFile
Document "1" *-- "0..1" Thumbnail

@enduml
```

## Repository Ports

### IDocumentRepository
Manages document metadata persistence in the database.

Operations:
- `AddAsync(Document)` - Create new document
- `UpdateAsync(Document)` - Update existing document
- `GetByIdAsync(long)` - Retrieve document by ID
- `GetAllAsync()` - Retrieve all documents
- `DeleteAsync(long)` - Delete document

### IOriginalFileRepository
Manages original file storage on the file system.

Operations:
- `SaveAsync(Stream, string)` - Save uploaded file
- `GetAsync(string)` - Retrieve file
- `DeleteAsync(string)` - Delete file

### IArchiveFileRepository
Manages archived file storage on the file system.

Operations:
- `SaveAsync(Stream, string)` - Save archived file
- `GetAsync(string)` - Retrieve file
- `DeleteAsync(string)` - Delete file

### IThumbnailFileRepository
Manages thumbnail storage on the file system.

Operations:
- `SaveAsync(Stream, string)` - Save thumbnail
- `GetAsync(string)` - Retrieve thumbnail
- `DeleteAsync(string)` - Delete thumbnail

## Hexagonal Architecture

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Container.puml

package "Domain Layer" {
  [Document Model] as DomainModel
  interface "IDocumentRepository" as IDocRepo
  interface "IOriginalFileRepository" as IOrigRepo
  interface "IArchiveFileRepository" as IArchRepo
  interface "IThumbnailFileRepository" as IThumRepo
}

package "Infrastructure Layer" {
  [DocumentRepository] as DocRepo
  [OriginalFileRepository] as OrigRepo
  [ArchiveFileRepository] as ArchRepo
  [ThumbnailFileRepository] as ThumRepo
  [FileSystemConfiguration] as FSConfig
  database "PostgreSQL" as DB
  folder "File System" as FS
}

package "Application Layer" {
  [Document Endpoints] as Endpoints
}

Endpoints --> IDocRepo
Endpoints --> IOrigRepo
Endpoints --> IArchRepo
Endpoints --> IThumRepo

IDocRepo <|.. DocRepo
IOrigRepo <|.. OrigRepo
IArchRepo <|.. ArchRepo
IThumRepo <|.. ThumRepo

DocRepo --> DB
OrigRepo --> FS
ArchRepo --> FS
ThumRepo --> FS

OrigRepo --> FSConfig
ArchRepo --> FSConfig
ThumRepo --> FSConfig

@enduml
```

## File Storage Strategy

Documents support three types of file attachments:
1. **Original File**: The originally uploaded file
2. **Archive File**: An archived/processed version of the document
3. **Thumbnail**: A preview image of the document

All files are stored on the file system with paths configured via `FileSystemConfiguration`. The configuration provides a base `Media` path, and each file type is stored in its respective subdirectory.

## Usage Scenarios

### Upload Document with File Only
User uploads a file which becomes the original file. The title is set from the filename, and other properties are null.

### Create Full Document
User provides complete metadata including title, document date, content, and uploads all three file types.

### Update Document
User can update any property including replacing existing files.

### View Document
User views document details and can preview PDF files in the browser using an integrated PDF viewer.

### Delete Document
User deletes a document which removes database entry and all associated files from the file system.
