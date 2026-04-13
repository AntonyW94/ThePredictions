# Football API Resilience

## Status

Not Started | **In Progress** | Complete

## Summary

Implement resilience patterns for the external Football API (api-sports.io) to handle failures gracefully and prevent cascading failures.

## Priority

**Critical** - Site completely fails if API unavailable

## Requirements

### Circuit Breaker Pattern
- [x] Implement Polly circuit breaker
- [x] Configure failure threshold
- [x] Configure recovery time

### Retry Logic
- [x] Add retry policies with exponential backoff
- [x] Configure maximum retry attempts
- [x] Handle transient failures

### Fallback/Caching
- [ ] Cache API responses for fallback data — _out of scope, see #11_
- [ ] Serve cached data when API unavailable — _out of scope, see #11_
- [ ] Display appropriate user messaging — _out of scope, see #11_

### Graceful Degradation
- [ ] Show cached/fallback data when external APIs fail — _out of scope, see #11_
- [ ] Inform users of reduced functionality — _out of scope, see #11_
- [x] Log failures for monitoring

## Technical Notes

Use Polly library for resilience:
- `Microsoft.Extensions.Http.Resilience` (built on Polly v8)
- Configured in `HttpClient` registration via `AddResilienceHandler`
- Settings class: `FootballApiResilienceSettings` (configurable via `appsettings.json` under `FootballApi:Resilience`)

### Resilience pipeline (in order)
1. **Retry** — up to 3 attempts with exponential backoff + jitter for 5xx, 408, 429, and transient HTTP errors
2. **Circuit breaker** — opens after 50% failure ratio (minimum 5 requests in 30s window), stays open for 30s
3. **Timeout** — 30s per-request timeout

### Structured logging
- Retry attempts logged at Warning level
- Circuit breaker state changes logged at Error (opened) and Information (half-opened, closed)

## Related Items

- #24 External API Resilience (Football API)
- #49 Circuit Breaker Pattern
- #79 Graceful Degradation
- #11 Caching (separate feature)
