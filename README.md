# PowerPositionReportGenerator

## Overview

The PowerPositionReportGenerator project extracts power trade data from the Axpo service, aggregates volumes per delivery period, and writes the results to CSV files on a configured schedule. This tool is designed to facilitate efficient data handling and reporting for power trades.

The project is composed of a small set of services designed for clear separation of concerns, ensuring maintainability and testability.

## Services

- **`PowerTradeService`**
  - A thin wrapper around the external `Axpo.PowerService` (DLL). Responsible for fetching trades for a given date via `GetTradesAsync(DateTime date)`.
  - Implemented behind the `IPowerTradeService` interface, allowing for easy replacement or mocking during tests.

- **`TradeAggregator`**
  - Implements `ITradeAggregator`. Takes a sequence of `PowerTrade` objects and aggregates volumes by `Period` into a `Dictionary<int, double>`, where the key is the period number and the value is the summed volume.
  - Designed for deterministic, in-memory aggregation.

- **`ExtractService`**
  - Implements `IExtractService`. Orchestrates the extract process:
    1. Converts `DateTime.UtcNow` to London time (tries `Europe/London` and falls back to `GMT Standard Time`).
    2. Uses `IPowerTradeService` to fetch trades for the target date.
    3. Aggregates results using `ITradeAggregator`.
    4. Writes the aggregated results using `ICsvWriter` into the configured `OutputFolder`.
    5. Logs start and completion information.

- **`PowerPositionScheduler`**
  - A `BackgroundService` that drives the periodic execution of the extract flow.
  - Reads `ExtractIntervalMinutes` from configuration, runs an initial extract, then uses a `PeriodicTimer` to run extracts on the configured interval.
  - Resolves `IExtractService` from a scope each run to ensure scoped  services are handled correctly.

## Interfaces

The project relies on a few small interfaces to decouple implementations and facilitate straightforward testing:

- **`IPowerTradeService`** — Abstraction for fetching trades.
- **`ITradeAggregator`** — Abstraction for aggregation logic.
- **`ICsvWriter`** — Abstraction for writing the final CSV output.
- **`IExtractService`** — Abstraction for orchestrating extract functionality.

## Configuration

Required configuration keys (e.g., in `appsettings.json` ):

- **`ExtractIntervalMinutes`** — Interval in minutes between extract runs (integer).
- **`OutputFolder`** — Path where CSV outputs will be written.

Example `appsettings.json` snippet:

```json
{
  "ExtractIntervalMinutes": "60",
  "OutputFolder": "./output"
}
```

## DI Registration Example

```csharp
  _ = services.AddScoped<IExtractService, ExtractService>();
        _ = services.AddScoped<IPowerTradeService, PowerTradeService>();
        _ = services.AddScoped<ITradeAggregator, TradeAggregator>();
        _ = services.AddScoped<ICsvWriter, CsvWriter>();

        _ = services.AddHostedService<PowerPositionScheduler>();
```

## Output

CSV files are written to the configured `OutputFolder`. Each file contains the aggregated volumes per period for the extract date/time. 

## Running

To run the application, follow these steps:

1. Restore and build with .NET 8.
2. Configure `appsettings.json` .
3. Run the app; the background service will perform an initial extract and then run periodically based on the `ExtractIntervalMinutes`.

## Notes
- All services are intentionally small and testable; prefer unit tests for `TradeAggregator` and integration tests for `ExtractService` with a mocked `IPowerTradeService` and `ICsvWriter`.



