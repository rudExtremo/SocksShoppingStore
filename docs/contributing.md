# Contributing

Thanks for your interest in improving SocksShoppingStore!

## Guidelines

- Language: code comments, commit messages, and PRs in English
- Style: concise, consistent naming, small focused changes
- Commits: Conventional Commits (e.g., `feat: add ru localization for products`)
- Tests: add tests when you introduce logic changes

## Development

- Clone and run locally with `dotnet run --project SocksShoppingStore`
- Keep `appsettings.json` defaults safe (limits, guard) for demos
- Prefer configuration over constants when adding toggles

## Pull Requests

- Describe the change, motivation, and risks
- Note any security or performance impact
- If UI, include screenshots or brief notes

## Tests & Coverage

- Run unit tests: `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj --filter TestCategory=Unit -c Release`
- Collect coverage (exclude views and Program): `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter TestCategory=Unit --collect:"XPlat Code Coverage" --settings coverlet.runsettings`
