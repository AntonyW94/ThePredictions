# Database Resilience

## Status

Not Started | In Progress | **Complete**

## Summary

Improve database connection resilience with proper pooling configuration and retry policies for transient failures.

## Priority

**High** - Currently using defaults

## Requirements

### Connection Pooling Configuration
- [x] Configure explicit pool size in connection string
- [x] Set minimum pool size
- [x] Set maximum pool size
- [x] Configure connection lifetime

### Retry Policies
- [x] Add retry policies for transient database failures
- [x] Configure exponential backoff
- [x] Handle specific SQL exceptions

## Implementation Details

### Connection Pooling (`SqlConnectionFactory`)
Connection string is enhanced via `SqlConnectionStringBuilder` with:
- `MinPoolSize = 5`
- `MaxPoolSize = 100`
- `ConnectRetryCount = 3` (built-in connection-level retry)
- `ConnectRetryInterval = 10` (seconds between connection retries)
- `LoadBalanceTimeout = 300` (connection lifetime in pool)

### Retry Policy (Polly v8)
`SqlRetryPolicy` wraps database operations with exponential back-off:
- Default: 3 retries with 2-second base delay (2s, 4s, 8s)
- Configurable via `SqlResilience` section in `appsettings.json`
- Handles transient SQL errors: timeouts (-2), deadlocks (1205), connection drops (64, 233), network errors (10053, 10054, 10060), and Azure SQL errors (40143, 40197, 40501, 40613, 49918, 49919, 49920)

### Files Changed
- `src/ThePredictions.Application/Data/ISqlRetryPolicy.cs` - retry policy interface
- `src/ThePredictions.Infrastructure/Data/Resilience/SqlRetryPolicy.cs` - Polly implementation
- `src/ThePredictions.Infrastructure/Data/Resilience/SqlRetryPolicyOptions.cs` - configuration POCO
- `src/ThePredictions.Infrastructure/Data/Resilience/SqlTransientFaultDetector.cs` - transient error detection
- `src/ThePredictions.Infrastructure/Data/SqlConnectionFactory.cs` - pooling configuration
- `src/ThePredictions.Infrastructure/Data/DapperReadDbConnection.cs` - retry on read queries
- `src/ThePredictions.Infrastructure/DependencyInjection.cs` - service registration

## Connection String Configuration

```
Server=...;Database=...;Min Pool Size=5;Max Pool Size=100;Connection Lifetime=300;
```

## Retry Configuration

```csharp
// Using Polly v8 ResiliencePipeline
var pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<Exception>(SqlTransientFaultDetector.IsTransient),
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential
    })
    .Build();
```

## Optional appsettings.json Override

```json
{
  "SqlResilience": {
    "MaxRetryCount": 3,
    "BaseDelaySeconds": 2
  }
}
```

## Related Items

- #54 Database Connection Pooling Configuration
- #56 Database Connection Resilience
