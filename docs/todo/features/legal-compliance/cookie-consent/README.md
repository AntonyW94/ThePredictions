# GDPR Cookie Consent Banner

## Status

Not Started | In Progress | **Complete**

## Summary

A cookie consent mechanism compliant with GDPR regulations. Users must be informed about cookie usage and given the ability to accept, reject, or customise their cookie preferences before non-essential cookies are set.

## Priority

**Critical** (from roadmap)

## Requirements

- [x] Cookie consent banner
- [x] Preference management
- [x] Cookie policy page

## Cookie inventory (audited 2026-04-28)

**Essential**
- `refreshToken` — HTTP-only auth refresh token (set by API after login / register / password reset / Google sign-in). 7-day expiry. Strictly necessary; not gated by consent.

**Non-essential**
- None today. We do not run Google Analytics, Plausible, or any tracking. The architecture (`IConsentBannerService.AnalyticsAllowed` / `MarketingAllowed`) is in place so any future analytics or marketing cookie must consult the consent service before being set.

**First-party functional storage (not cookies, not consent-gated)**
- `localStorage` keys: `accessToken`, `themePreference`, `cookieConsent`.
