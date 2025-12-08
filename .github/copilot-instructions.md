# Repository Structure

Copilot must respect and leverage the following folder structure:

```
/.github            → CI, workflows, automation
/docs               → arc42-based documentation
                     - Each arc42 section is a folder
                     - Each topic is its own markdown file
                     - Diagrams are written using PlantUML
/src                → Productive application code
                     - Backend: .NET (C#)
                     - Frontend: Angular + TypeScript
/tests              → All tests
                     - Test projects for each backend project
                     - ArchitectureTests (backend architecture tests)
                     - Central.AcceptanceTests (Reqnroll + Aspire acceptance tests)
apphost.cs          → Aspire single-file host used only for local development
```

### Documentation Rules

* All architecture documentation follows the arc42 structure.
* Use PlantUML (`*.puml`) for diagrams.
* Do not embed large diagrams in code comments.

### Backend Rules

* Use C# and .NET.
* Follow the patterns already established under `/src`.

### Frontend Rules

* Use Angular + TypeScript.
* Use Angular best practices.
* Component/service tests live in `.spec.ts` files next to the component.

### Testing Rules

* Backend unit tests use **xUnit v3** + **AwesomeAssertions**.
* Architecture tests belong in `/tests/ArchitectureTests`.
* Acceptance tests belong in `/tests/Central.AcceptanceTests` using Reqnroll.
* Each backend project has a matching test project inside `/tests`.

---

# Development Flow

Copilot must follow this sequence when generating or modifying code:

## 1. Plan and document the architecture

* Update or create arc42 documentation in `/docs`.
* Use PlantUML diagrams when visualizations are needed.

## 2. Write a Cucumber/Reqnroll acceptance test first

* Place acceptance tests in `/tests/Central.AcceptanceTests`.
* Use Gherkin syntax and Reqnroll step definitions.

## 3. Implement the feature

* Add or update code in `/src`.
* Follow existing project patterns.

## 4. Write unit tests

* Backend: place tests in the matching project under `/tests`.
* Frontend: create/update `.spec.ts` files next to the Angular component.

## 5. Validate that all tests pass

## 6. Validate that code coverage is at least 80%

## 7. Validate `dotnet format`

## 8. Validate that frontend has no format or lint issues

## 9. Validate that backend and frontend builds have no errors or warnings

---

# Committing Flow

## 1. Keep commits as small as possible

* One logical change per commit.
* Do not mix unrelated changes.

## 2. Commit messages follow Conventional Commits

Examples:

* `feat: add new booking workflow`
* `fix: correct null reference in user handler`
* `test: add acceptance test for login`
* `docs: update arc42 building block view`
* `chore: apply dotnet format`

Forbidden:

* “update”
* “fix stuff”
* “misc changes”

---

# Additional Copilot Behavior Guidelines

### Code Quality

* Generate maintainable, clean code.
* Avoid unused code or commented-out blocks.
* Only generate documentation comments when helpful.

### Tests

* Use Arrange/Act/Assert.
* Backend tests use AwesomeAssertions.
* Frontend tests use Angular testing utilities and Jasmine expectations.

### Diagrams

* All diagrams must use PlantUML:

```
@startuml
...
@enduml
```

### Architecture Tests

* Follow existing patterns for architecture validation.
* Use ArchitectureTests project under `/tests/Central.ArchitectureTests`.

### Code generation

Always use context7 when I need code generation, setup or configuration steps, or
library/API documentation. This means you should automatically use the Context7 MCP
tools to resolve library id and get library docs without me having to explicitly ask.