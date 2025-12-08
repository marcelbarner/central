# Internationalization

## ngx-translate

The Angular client supports multiple languages using ngx-translate.

* **Translation Files**: JSON files in `public/i18n/` directory
* **Language Switching**: Dynamic language selection at runtime
* **Default Language**: Configurable fallback language
* **Usage**: Pipes and directives in templates, TranslateService in components

## Translation Management

```typescript
// Template usage
{{ 'HELLO.TITLE' | translate }}

// Component usage
this.translate.get('HELLO.MESSAGE').subscribe(text => {
  // Use translated text
});
```
