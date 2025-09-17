# Technical Debt

- Catalog price validation UX
  - Current: number inputs accept browser-level validation; no explicit error messages/highlights for invalid/min>max.
  - Plan: add client/server validation with messages and red highlights; cover M-11/M-12 precisely.
- Cart decrement-to-zero UX
  - Current: quantity input clamps to >=1; delete via trash.
  - Plan: consider UX to auto-delete on decrement at 1 (or show confirm); adjust tests accordingly.
- UI coverage in CI
  - Add dedicated UI coverage collection (instrumented app) — initial step wired for main branch.
