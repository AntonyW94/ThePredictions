# Fix DapperUserStore Not Persisting PreferredTheme

## Status

**Not Started** | In Progress | Complete

## Priority

**High** (silent data loss bug)

## Summary

`DapperUserStore.CreateAsync` and `DapperUserStore.UpdateAsync` do not include the `[PreferredTheme]` column in their SQL. The `ApplicationUser` model has the property, and `UpdateThemePreferenceCommandHandler` sets it then calls `userManager.UpdateAsync`, but the change is silently dropped on persistence — every theme change reverts on next page load that re-reads from the database.

## Requirements

- [ ] Add `[PreferredTheme] = @PreferredTheme` to the UPDATE SQL in [`DapperUserStore.UpdateAsync`](../../../../src/ThePredictions.Infrastructure/Identity/DapperUserStore.cs)
- [ ] Add `[PreferredTheme]` to the column list and `@PreferredTheme` to the VALUES list in `DapperUserStore.CreateAsync`
- [ ] Verify with a manual smoke test that theme changes persist across page reloads
- [ ] No schema change required — the column already exists with default `'light'`

## Technical Notes

- This bug has been in the codebase since `PreferredTheme` was added to `ApplicationUser`. Symptoms: the in-memory user object reflects the new theme for the rest of the request, but the next request reads the old value back from the DB.
- Reproduction: change theme in account settings → reload page → theme reverts.
