# Persistence Options

The app supports two repository providers for products:

- In-memory (default): wraps the legacy static list, no persistence
- JSON file: stores products in a JSON file with simple auto-increment IDs

## Configure

In `SocksShoppingStore/appsettings.json`:

```
"Repository": {
  "Provider": "InMemory" | "Json",
  "Json": {
    "Path": "App_Data/products.json"
  }
}
```

- Relative paths are resolved against the content root
- On first use, the JSON repository seeds from the legacy in-memory list

## DI Registration

`Program.cs` wires `IProductRepository` based on config. Controllers consume the interface and work unchanged.

