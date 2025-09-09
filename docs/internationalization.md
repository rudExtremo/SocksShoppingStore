# Internationalization (i18n)

The app supports English and Russian UI. Product names and descriptions are localized through a simple in-memory map.

## Cultures

- Supported UI cultures: `en`, `ru`
- Default culture: `en` (numbers use `en-US`)

## Switching Language

- Query string: append `?culture=en` or `?culture=ru`
- Cookie: POST to `/Localization/Set` with form fields `culture` and `returnUrl`

## Product Localization

- `ProductCatalogLocalizer` maps IDs to localized name/description for RU
- EN is the default (source values in `ProductRepository`)

## Resources

- UI localization scaffolding is enabled with `Resources` path; add resource files if needed

