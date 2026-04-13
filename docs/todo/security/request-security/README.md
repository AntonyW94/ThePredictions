# Request-Level Security Headers

## Status

Not Started | In Progress | **Complete**

## Summary

Comprehensive HTTP security headers to protect against common web vulnerabilities including clickjacking, XSS, and content injection attacks.

## Priority

**Medium** (from roadmap)

## Requirements

- [x] Review CSP headers
- [x] Implement HSTS (1 year with subdomains)
- [x] Add X-Frame-Options (DENY)
- [x] Configure CORS properly (restricted origins, specific methods and headers)

## Implementation Notes

Implemented in `SecurityHeadersMiddleware.cs` with the following headers:
- **CSP**: Full policy with `wasm-unsafe-eval` for Blazor WebAssembly
- **HSTS**: 1 year, includes subdomains
- **X-Frame-Options**: DENY
- **X-Content-Type-Options**: nosniff
- **Permissions-Policy**: Restricts camera, geolocation, microphone, payment, USB
- **Referrer-Policy**: strict-origin-when-cross-origin
- **X-XSS-Protection**: 1; mode=block

Rate limiting also configured in `DependencyInjection.cs`:
- Global: 100 requests/minute per IP
- Auth endpoints: 10 requests/5 minutes per IP
- API endpoints: 60 requests/minute per IP
