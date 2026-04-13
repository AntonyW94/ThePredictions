# Account Lockout After Failed Attempts

## Status

Not Started | In Progress | **Complete**

## Summary

Account lockout functionality to protect user accounts from brute-force attacks by temporarily locking accounts after multiple failed login attempts.

## Priority

**Medium** (from roadmap)

## Requirements

- [x] Lock account after 5 failed login attempts
- [x] Configurable lockout duration (15 minutes)
- [ ] Unlock via email
- [ ] Admin unlock capability

## Implementation Notes

Core lockout is configured via ASP.NET Identity in `DependencyInjection.cs`:
- 5 failed attempts triggers lockout
- 15-minute lockout duration
- Email unlock and admin unlock are not yet implemented but are low priority given the automatic unlock after 15 minutes
