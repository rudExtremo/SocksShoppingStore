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

## Testing Strategy and Execution

The optimal strategy is a hybrid approach to test execution, balancing rapid feedback with resource constraints (especially the GitHub Actions free tier minute limits).

### Test Environments

| Criteria          | Local Machine                       | GitHub Actions Hosted Runner        |
| :---------------- | :---------------------------------- | :---------------------------------- |
| **Cost**          | None (except hardware)              | Free up to 2,000 min/month, then paid |
| **Execution Time**| Unlimited                           | Limited (6 hours per job)           |
| **Consistency**   | Low ("works on my machine")         | High (clean environment per run)    |
| **Debugging**     | Easy (IDE integration)              | Complex (via logs)                  |
| **Feedback Loop** | Very fast                           | Fast, but with queue/startup delay  |

### Recommended Test Distribution

| Test Type                  | Recommended Primary Environment | Frequency                       |
| :------------------------- | :------------------------------ | :------------------------------ |
| **Unit Tests**             | GitHub Actions                  | On every commit/PR              |
| **Integration Tests**      | GitHub Actions                  | On every commit/PR              |
| **API Tests (Smoke)**      | GitHub Actions                  | On every commit/PR              |
| **UI Tests (Smoke)**       | GitHub Actions                  | On every commit/PR              |
| **Performance Tests (k6)** | GitHub Actions                  | On every commit/PR              |
| **Security Scans (ZAP)**   | GitHub Actions                  | On every commit/PR              |
| **UI Tests (Full Regression)** | Local Machine               | Before major merges, on a schedule |

### Running Tests

*   **Run fast CI suite (Unit & Integration):**
    ```shell
    dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj --filter "TestCategory=Unit|TestCategory=Integration" -c Release
    ```
*   **Run all tests locally (including full UI regression):**
    ```shell
    dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release
    ```
*   **Collect code coverage:**
    ```shell
    dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter "TestCategory=Unit|TestCategory=Integration" --collect:"XPlat Code Coverage" --settings coverlet.runsettings
    ```
