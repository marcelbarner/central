---
applyTo: "src/Central.Client/**/*"
---

# Project Structure Rules

This Angular application follows the conventions of the **ng-matero** project template:

```
/src/Central.Client
└── src/app
    ├── routes/                → Routed pages/components
    ├── shared/
    │   ├── components/        → Reusable UI components
    │   ├── services/          → Reusable services
    │   ├── pipes/             → Reusable pipes
    │   └── directives/        → Reusable directives
    └── core/
        └── states/            → NGXS states
```

Copilot must strictly follow these placement rules:

### Routed Components

* Any component that is reachable via Angular routing **must** be placed under:

  ```
  src/app/routes/
  ```

### Shared Items

Reusable elements (not route-accessible pages) must go under:

```
src/app/shared/components/
src/app/shared/services/
src/app/shared/pipes/
src/app/shared/directives/
```

### State Management (NGXS)

* All NGXS states must be placed in:

  ```
  src/app/core/states/
  ```
* Follow NGXS best practices:

  * Define actions in their own classes.
  * Keep selectors pure.
  * Keep side effects inside NGXS `@Action()` handlers or services.
  * Do not put HTTP logic directly inside components.

---

# Development Rules

## Angular Best Practices

Copilot must follow standard Angular + ng-matero best practices:

* Use **standalone components** if the project already uses them; otherwise follow the existing app pattern.
* Use the Angular CLI structure and naming conventions.
* Use `OnPush` change detection for performance unless the app structure dictates otherwise.
* Keep components thin; put logic into services.
* Use RxJS best practices (avoid deep nested subscriptions; prefer `async` pipe).
* Follow strict typing everywhere (no `any`).

## Internationalization (i18n)

The application supports multiple languages using **ngx-translate**:

* **Translation Files**: All translation JSON files are located in `public/i18n/`
* **Usage in Templates**: Use the `translate` pipe: `{{ 'KEY.NAME' | translate }}`
* **Usage in Components**: Inject `TranslateService` and use `.get()` or `.instant()`
* **Translation Keys**: Use SCREAMING_SNAKE_CASE with dot notation for nesting (e.g., `HELLO.TITLE`, `ERROR.NOT_FOUND`)
* **New Features**: Always add translation keys for all user-facing text—never hardcode strings in templates or components
* **Consistency**: Ensure all similar UI elements use consistent translation key patterns

---

# Styling Rules

### Global Styling Preferences

Styling is done with **SCSS**. When using UI components or styles, use packages in the following priority order:

1. `@ng-matero/extensions`
2. `@angular/material`

When generating component templates or layout:

* Prefer ng-matero UI components first.
* If unavailable, fall back to Angular Material.
* Ensure visual and structural consistency across similar pages for a unified UX.

### Style Conventions

* Use BEM-like naming where appropriate.
* Keep styles local to components via Angular’s SCSS encapsulation.
* Avoid inline CSS.
* Do not generate unused style rules.

---

# Testing Rules

### Unit Tests

* All frontend unit tests must be written in `.spec.ts` files next to the component/service/directive/pipe.
* Follow Angular TestBed and Jasmine best practices.
* Ensure tests cover:

  * Inputs/outputs
  * DOM behavior
  * Service calls and interactions
  * NGXS state behaviors (selectors, actions, state transitions)

### Coverage Requirement

* Copilot must produce implementations and tests that help maintain **≥ 80% code coverage**.

---

# Validation Requirements

Copilot should encourage and assume the following checks:

### Build

* The Angular project must build without warnings or errors.

### Linting

* Code must pass linting (Angular ESLint / project config).
* Follow existing lint rules (naming, imports, spacing, max-line rules, etc.).

### Tests

* The entire test suite must pass.
* Coverage remains ≥ 80%.

---

# UI/UX Consistency

* Pages with similar purpose or structure must share similar layout, spacing, and component usage.
* Use shared components whenever possible.
* Keep typography, spacing, alignment, and interaction patterns consistent across the app.
* Do not introduce one-off UI patterns unless necessary.

---

# Additional Copilot Behavior Guidelines

### Code Quality

* Generate minimal, clean, maintainable code.
* Avoid commented-out blocks.
* Only create documentation comments when helpful.
* Prefer pure, small functions.
* Prefer observable streams over imperative patterns when reasonable.

### File Naming

* Use Angular conventions:
  `feature-name.component.ts`
  `feature-name.service.ts`
  `feature-name.state.ts`
  `feature-name.actions.ts`

### Imports

* Organize imports consistently.
* Avoid unused imports.
* Respect the layered structure: shared → routes; never routes → shared.
