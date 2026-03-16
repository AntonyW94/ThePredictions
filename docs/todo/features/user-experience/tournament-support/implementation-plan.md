# Tournament Support - Implementation Plan

## Overview

This is the actionable implementation plan for adding tournament support to ThePredictions, targeting the FIFA World Cup 2026 (starting June 2026). This plan was derived from the [feature plan](README.md) and updated in March 2026 after a codebase review and detailed technical discussion.

**Target Delivery:** May 2026
**Scope:** Pure tournament format (World Cup). Champions League hybrid format is deferred.
**Builder:** Claude (AI Assistant) via Claude Desktop app
**Database Migrations:** Manual SQL scripts run against production via SSMS

---

## BEFORE YOU START - API Data Verification Required

> **This must be done first in a Claude Desktop session where API calls are possible.**
> **The API key (`FootballApi-ApiKey`) is stored in Azure Key Vault. The owner will provide it in the Claude Desktop session.**
> **Do NOT ask for the API key in Claude Code web sessions - it is not available there.**

### What to call

| Endpoint | Purpose |
|----------|---------|
| `fixtures/rounds?league=1&season=2026` | Get all World Cup 2026 round name strings |
| `fixtures?league=1&season=2026` | Get all fixtures with team/date/round data |
| `teams?league=1&season=2026` | Get all 48 teams |
| `fixtures/rounds?league=1&season=2022` | Fallback: get 2022 World Cup round names if 2026 not ready |
| `fixtures?league=1&season=2022` | Fallback: get 2022 fixtures if 2026 not ready |

### What to verify before continuing

1. [ ] **Round name patterns** - Confirm the exact strings returned (e.g. "Group A - 1", "Quarter-finals", "3rd Place Final"). The sync parser depends on these exact patterns.
2. [ ] **2026 vs 2022 format** - 2026 has 48 teams in 12 groups (A-L) with a Round of 32. Verify the API reflects this new format, or if it still shows the old 32-team structure.
3. [ ] **Knockout match team data** - Do knockout fixtures return `null` team IDs before the preceding stage is complete? Or do they return placeholder names? Or are knockout fixtures not created until teams are known?
4. [ ] **TBC team representation** - Check if `fixture.teams.home` is `null`, `0`, or has a placeholder like `{ "id": null, "name": "Winner Group A" }`.
5. [ ] **Match count per group matchday** - Confirm 2 matches per group per matchday (not 3 or 4). With 12 groups x 2 matches = 24 matches per matchday.
6. [ ] **Semi-finals + Final timing** - Check the date gaps between semi-finals and final/3rd place to confirm they'd naturally be grouped.
7. [ ] **Any unexpected round names** - Check for "Playoff" or other stages not accounted for in the parser.

### If 2026 data is not available

Use 2022 World Cup data (League ID: 1, Season: 2022) to validate the approach, but note the structural differences:
- 2022: 32 teams, 8 groups (A-H), no Round of 32
- 2026: 48 teams, 12 groups (A-L), includes Round of 32

Document any findings and adjust the round name parser accordingly before proceeding to Phase 1.

---

## Decisions Log

| Decision | Outcome | Date |
|----------|---------|------|
| TournamentStages table | **Dropped.** C# enum used only during sync for grouping logic. Not persisted to DB. Generates the `DisplayName` on Round. | Mar 2026 |
| TournamentGroups table | **Dropped entirely.** Not stored, not displayed. Group info is irrelevant to the prediction game. | Mar 2026 |
| Multiple ApiRoundNames on Round | **Dropped.** Each Match stores its own `ApiRoundName`. Round keeps existing single `ApiRoundName` (null for grouped tournament rounds). | Mar 2026 |
| DisplayName on Round | **Added as non-nullable column.** Set for ALL rounds: "Gameweek {N}" for leagues, descriptive names for tournaments ("Group Stage - Matchday 1", "Quarter-finals"). All UI/API reads use DisplayName instead of RoundNumber. RoundNumber becomes internal ordering/unique constraint only. | Mar 2026 |
| TournamentStage enum | C# enum in code only. Used during sync to decide grouping and generate DisplayName. Not persisted to any DB column. | Mar 2026 |
| Per-match deadlines | **NOT default.** Standard round deadline (30 min before first match) applies to all matches. `CustomLockTimeUtc` only set on matches from later stages in combined rounds (e.g. Final + 3rd place in a round that also contains semi-finals). | Mar 2026 |
| TBC teams | Nullable `HomeTeamId`/`AwayTeamId` already supported. Users CANNOT predict a match until both teams are known. | Mar 2026 |
| Combined round unlocking | When stages are combined (e.g. SF + Final/3rd place), semi-final predictions lock at round deadline. Once semis finish and final teams are known, users can go back to predict the Final/3rd place. Those matches get `CustomLockTimeUtc` = 30 min before first of final/3rd place. | Mar 2026 |
| Round grouping | **Fully automatic** during sync. No admin involvement. System follows rules to group matches. | Mar 2026 |
| Minimum matches per round | **Hard minimum of 4.** Fewer matches means too many users end up on the same points. Semi-finals (2) + Final (1) + 3rd place (1) = 4, combined into one round. | Mar 2026 |
| Scoring | 90 minutes only. Extra time and penalties do not count. | Mar 2026 |
| Entry deadline | Same as leagues. Admin sets it (typically 1 day before first match). Cannot join after any match has started. | Mar 2026 |
| Rescheduling | Match time updates on sync. Existing predictions remain valid. | Mar 2026 |
| Group tables / standings | Not displayed. Out of scope. | Mar 2026 |
| UI differentiation | Coloured badge/chip on cards to distinguish league vs tournament. | Mar 2026 |
| Prize types for tournaments | Overall, Round, MostExactScores only. Monthly prizes hidden for tournament competitions. | Mar 2026 |
| Boosts in tournaments | Same as leagues - boosts work identically. | Mar 2026 |
| Champions League / Hybrid support | Deferred - not in scope for May deadline. | Mar 2026 |
| Database migrations | Manual SQL scripts via SSMS. | Mar 2026 |

---

## Tournament Rules Summary (Plain English)

1. A tournament (like the World Cup) has a group stage followed by knockout rounds. The system automatically imports all matches from the football API and groups them into prediction rounds.

2. **Group stage grouping:** All group matchday 1 matches across all groups become one prediction round called "Group Stage - Matchday 1". Same for matchdays 2 and 3. This gives ~24 matches per round.

3. **Knockout grouping:** Each knockout stage becomes its own prediction round (Round of 32 = one round, Round of 16 = one round, Quarter-finals = one round). If a stage has fewer than 4 matches, it's combined with the next stage.

4. **Combined round example:** Semi-finals (2 matches) + Final (1) + 3rd Place Playoff (1) = 4 matches in one round called "Semi-finals, Final & Third Place Playoff".

5. **Deadlines:** All predictions for a round must be submitted 30 minutes before the first match in that round kicks off. This is the same model as leagues.

6. **Combined round exception:** When stages are combined (e.g. Semi-finals + Final/3rd place), the semi-final predictions lock at the round deadline (30 min before first semi). The Final and 3rd place matches show "TBC" for teams until the semis finish. Once both teams are known, users can go back and predict them. Those matches have their own `CustomLockTimeUtc` set to 30 minutes before the first of the final/3rd place kicks off.

7. **TBC matches:** A match can exist with unknown teams (shown as "TBC"). Users cannot submit predictions for these matches. Once both teams are confirmed (after the preceding stage completes and the API returns real team IDs), the match unlocks for predictions.

8. **Scoring:** Standard prediction scoring (exact score = max points, correct result = fewer points). Only the 90-minute score counts - extra time and penalties are ignored.

9. **Prizes:** Overall (best across tournament), Round (best per prediction round), and Most Exact Scores. No Monthly prizes.

10. **Joining:** Users must join before the entry deadline (set by admin, typically 1 day before the first match). Cannot join after any match has started.

11. **Boosts:** Work the same as leagues - applied per prediction round.

---

## Codebase Review Summary (March 2026)

### Already in place (no work needed)

| Feature | Location | Notes |
|---------|----------|-------|
| `Match.CustomLockTimeUtc` | Domain model + DB | Per-match deadline column, nullable, unused |
| `Match.PlaceholderHomeName/AwayName` | Domain model + DB | Nullable string columns, unused |
| `Match.HomeTeamId/AwayTeamId` nullable | Domain model + DB | Supports TBD teams |
| `Seasons.CompetitionType` column | DB only (default 0) | Exists in schema, NOT in domain model |
| `NumberOfRounds` validation 1-52 | Validator | Fine for tournaments (~7 rounds) |

### Gaps confirmed (work required)

| Gap | Impact |
|-----|--------|
| `CompetitionType` not in Season domain model, factory, constructor, DTOs, or admin UI | Cannot create/identify tournament seasons |
| `MatchPredictionDto` missing placeholder names, CustomLockTimeUtc, lock status | UI cannot display tournament match info |
| `GetPredictionPageDataQueryHandler` hard-joins Teams with no COALESCE fallback | Query fails for null team IDs |
| API sync handler has no tournament round parsing | Cannot sync World Cup fixtures |
| `Round` entity missing `DisplayName` | No user-friendly round names |
| `Match` entity missing `ApiRoundName` | No match-level tournament context |
| `PredictionDomainService` has no per-match deadline logic | Combined round matches all share round deadline |
| `DatabaseRefresher.cs` needs awareness of new columns | Dev DB refresh would miss tournament data |
| Prize filtering not competition-type-aware | Monthly prizes would appear for tournaments |

---

## Implementation Phases

### Phase 1: Foundation - Database & Domain (Week 1-2)

**Goal:** Database schema updated, domain models extended, Season admin supports competition type.

#### Step 1.1: Database Schema Changes

**Manual step (owner runs SQL against dev DB first, then prod):**

SQL scripts to create:

1. [ ] `ALTER [Rounds]` - add `DisplayName` (nvarchar(200) NOT NULL DEFAULT '')
2. [ ] Backfill existing rounds: `UPDATE [Rounds] SET [DisplayName] = 'Gameweek ' + CAST([RoundNumber] AS NVARCHAR)`
3. [ ] `ALTER [Matches]` - add `ApiRoundName` (nvarchar(128) nullable)
4. [ ] Add `DEFAULT 0` constraint on `Seasons.CompetitionType` if not already present

**Claude builds:** SQL scripts ready to run. Owner executes manually.

#### Step 1.2: Update database-schema.md

5. [ ] Update `Rounds` table: add `DisplayName` column
6. [ ] Update `Matches` table: add `ApiRoundName` column

#### Step 1.3: New Domain Enumerations

7. [ ] Create `CompetitionType.cs` enum (League = 0, Tournament = 1)
8. [ ] Create `TournamentStage.cs` enum (used only in sync logic, not persisted)
   - Values: Group, RoundOf32, RoundOf16, QuarterFinals, SemiFinals, ThirdPlace, Final

#### Step 1.4: Update Season Domain Model

9. [ ] Add `CompetitionType` property to `Season`
10. [ ] Update `Season` constructor to accept `CompetitionType`
11. [ ] Update `Season.Create()` factory to accept `CompetitionType`
12. [ ] Update `Season.UpdateDetails()` to accept `CompetitionType`
13. [ ] Add helper: `IsTournament => CompetitionType == CompetitionType.Tournament`
14. [ ] Unit tests for Season with CompetitionType

#### Step 1.5: Update Round Domain Model

15. [ ] Add `DisplayName` (string, non-nullable) property to `Round`
16. [ ] Update `Round` constructor to accept `DisplayName`
17. [ ] Update `Round.Create()` factory to accept `DisplayName`
18. [ ] Update `Round.UpdateDetails()` to accept `DisplayName`
19. [ ] Update all code that reads `RoundNumber` for display purposes to use `DisplayName`
20. [ ] Update existing sync handler to set `DisplayName` = "Gameweek {RoundNumber}" for league rounds
21. [ ] Unit tests for new Round DisplayName property

#### Step 1.6: Update Match Domain Model

22. [ ] Add `ApiRoundName` (string?) property to `Match`
23. [ ] Update `Match` constructor to accept `ApiRoundName`
24. [ ] Add `GetEffectiveDeadline(DateTime roundDeadline)` method - returns `CustomLockTimeUtc ?? roundDeadline`
25. [ ] Add `IsPredictionLocked(DateTime utcNow, DateTime roundDeadline)` method
26. [ ] Add `AreTeamsConfirmed` property - `HomeTeamId.HasValue && AwayTeamId.HasValue`
27. [ ] Add `AssignTeams(int homeTeamId, int awayTeamId)` method (sets team IDs, clears placeholder names)
28. [ ] Unit tests for all new Match methods

#### Step 1.7: Update Contracts & DTOs

29. [ ] Add `CompetitionType` to `BaseSeasonRequest` / `SeasonDto`
30. [ ] Add `DisplayName` to any Round DTOs
31. [ ] Update `CreateSeasonRequestValidator` for CompetitionType

#### Step 1.8: Update Application Layer

32. [ ] Update `CreateSeasonCommand` / handler to include CompetitionType
33. [ ] Update `UpdateSeasonCommand` / handler to include CompetitionType
34. [ ] Update `GetSeasonByIdQuery` / `FetchAllSeasonsQuery` to return CompetitionType
35. [ ] Update all round-related queries to return DisplayName

#### Step 1.9: Update Season Admin UI

36. [ ] Add CompetitionType dropdown to Season create/edit pages
37. [ ] Default to "League" for existing behaviour

#### Step 1.10: Run Tests & Coverage

38. [ ] Run unit tests - ensure 100% coverage maintained on Domain project
39. [ ] Run `coverage-unit.bat` to verify

**Deliverables:** Database updated, domain models extended, all rounds have DisplayName, Season admin shows competition type.

---

### Phase 2: Tournament API Sync (Week 3-4)

**Goal:** World Cup seasons can be synced from the API. Matches auto-grouped into prediction rounds.

#### Step 2.1: Tournament Round Name Parser

1. [ ] Create helper method/class to parse tournament API round names:
   - "Group A - 1" -> TournamentStage.Group, groupLetter='A', matchday=1
   - "Round of 32" -> TournamentStage.RoundOf32
   - "Round of 16" -> TournamentStage.RoundOf16
   - "Quarter-finals" -> TournamentStage.QuarterFinals
   - "Semi-finals" -> TournamentStage.SemiFinals
   - "3rd Place Final" -> TournamentStage.ThirdPlace
   - "Final" -> TournamentStage.Final
2. [ ] Unit tests for all round name patterns (including edge cases from API verification)

#### Step 2.2: Auto-Grouping Logic

3. [ ] Implement automatic round grouping rules:
   - **Group stage:** All matches with same matchday number = one round. DisplayName = "Group Stage - Matchday {N}"
   - **Large knockout stages (>=4 matches):** Own round. DisplayName = stage name (e.g. "Round of 32", "Quarter-finals")
   - **Small knockout stages (<4 matches):** Combine consecutive stages until >= 4 matches. DisplayName = combined name (e.g. "Semi-finals, Final & Third Place Playoff")
4. [ ] Implement deadline calculation:
   - Round deadline = 30 min before earliest match in the round
   - For combined rounds: `CustomLockTimeUtc` set on matches from later stages = 30 min before earliest match in that sub-stage
5. [ ] Unit tests for grouping logic with various stage sizes

#### Step 2.3: Update SyncSeasonWithApiCommandHandler

6. [ ] Branch sync logic based on `season.CompetitionType`:
   - League: existing logic unchanged
   - Tournament: new tournament sync path
7. [ ] Tournament sync flow:
   a. Fetch all fixtures and round names from API
   b. Parse each round name to determine stage and matchday
   c. Apply auto-grouping rules to create prediction rounds
   d. Create Round records with appropriate DisplayName and deadline
   e. Create Match records with `ApiRoundName` set
   f. For matches with null team IDs: set `PlaceholderHomeName`/`PlaceholderAwayName` from API data
   g. For combined rounds: set `CustomLockTimeUtc` on later-stage matches
8. [ ] Handle re-sync (subsequent syncs after initial):
   - Update match times if changed
   - Assign teams when API returns real team IDs for previously-TBC matches
   - Set `CustomLockTimeUtc` on newly-assigned matches in combined rounds
9. [ ] Unit tests for tournament sync logic

#### Step 2.4: Tests

10. [ ] Unit tests for round name parsing (all patterns)
11. [ ] Unit tests for auto-grouping with World Cup structure
12. [ ] Unit tests for combined round deadline logic
13. [ ] Verify 100% coverage

**Deliverables:** Tournament sync working with automatic round grouping. No admin involvement needed.

---

### Phase 3: Prediction Logic & UI for Tournaments (Week 4-5)

**Goal:** Users can predict tournament matches. Per-match locking works for combined rounds. TBC matches handled.

#### Step 3.1: Domain Service Update

1. [ ] Update `PredictionDomainService.SubmitPredictions` to handle tournament rules:
   - Reject predictions for matches where `AreTeamsConfirmed` is false
   - For matches with `CustomLockTimeUtc`: check match-level deadline
   - For matches without `CustomLockTimeUtc`: check round deadline (standard behaviour)
2. [ ] Unit tests for: TBC match rejection, combined round with mixed locked/unlocked matches

#### Step 3.2: Update Prediction Query

3. [ ] Update `GetPredictionPageDataQueryHandler` SQL:
   - Use LEFT JOIN for Teams (not INNER JOIN) to handle null team IDs
   - Add COALESCE: `COALESCE(ht.[Name], m.[PlaceholderHomeName]) AS HomeTeamName`
   - Include `m.[CustomLockTimeUtc]`, `m.[PlaceholderHomeName]`, `m.[PlaceholderAwayName]`
   - Calculate `IsLocked` and `EffectiveDeadlineUtc` in mapping
   - Include `m.[HomeTeamId]`, `m.[AwayTeamId]` to detect TBC matches
4. [ ] Return `AreTeamsConfirmed` flag per match

#### Step 3.3: Update DTOs

5. [ ] Add to `MatchPredictionDto`: `CustomLockTimeUtc`, `EffectiveDeadlineUtc`, `IsLocked`, `PlaceholderHomeName`, `PlaceholderAwayName`, `AreTeamsConfirmed`
6. [ ] Add to `PredictionPageDto`: `IsTournament`, `CompetitionType`
7. [ ] Add `DisplayName` to any round DTOs used in the UI

#### Step 3.4: Update Prediction UI

8. [ ] Show match lock indicator badge on prediction cards when match has CustomLockTimeUtc
9. [ ] Show individual countdown/deadline per match when CustomLockTimeUtc differs from round deadline
10. [ ] Disable prediction inputs for locked matches (show as read-only with existing predictions)
11. [ ] Disable prediction inputs for TBC matches (show "TBC" for team names, greyed out)
12. [ ] Display placeholder team names when real team names are null
13. [ ] Use `DisplayName` instead of "Gameweek {N}" everywhere in the UI

#### Step 3.5: Tests & Coverage

14. [ ] Unit tests for domain service deadline logic
15. [ ] Verify 100% coverage

**Deliverables:** Users can predict tournament matches. Combined round locking works. TBC matches shown but not predictable.

---

### Phase 4: UI Polish, Prizes & Competition Badge (Week 5-6)

**Goal:** Full tournament flow polished. Prizes correct. League/tournament visually distinct.

#### Step 4.1: Competition Type Badge

1. [ ] Add coloured badge/chip to season cards, round cards, and league cards:
   - **League:** Blue badge with table/list icon - "League"
   - **Tournament:** Amber/gold badge with trophy icon - "Tournament"
2. [ ] Use Bootstrap Icons (`bi-trophy`, `bi-table`) for badge icons
3. [ ] CSS for badge styling (pill-shaped, consistent sizing)

#### Step 4.2: Dashboard Updates

4. [ ] Show `DisplayName` on round cards (not "Gameweek {N}")
5. [ ] Show "TBC" badge for placeholder teams in upcoming rounds
6. [ ] Show competition type badge on season/league cards

#### Step 4.3: Prize Filtering

7. [ ] Filter out Monthly prize type when league's season has `CompetitionType = Tournament`
8. [ ] Update prize configuration UI to hide Monthly option for tournament leagues
9. [ ] Update prize processing logic to skip Monthly for tournaments
10. [ ] Verify Overall, Round, and MostExactScores work correctly with tournament round structure

#### Step 4.4: Mobile Responsiveness

11. [ ] Test tournament UI on mobile viewports
12. [ ] Ensure lock badges and TBC indicators don't break layout

#### Step 4.5: Integration Testing

13. [ ] End-to-end test: create tournament season -> sync from API -> verify auto-grouped rounds -> submit predictions -> enter results -> view leaderboard
14. [ ] Test with World Cup 2022 API data (League ID: 1, Season: 2022) if 2026 not available
15. [ ] Test combined round flow: predict semis -> lock -> teams confirmed -> predict final/3rd place -> lock
16. [ ] Test that league seasons are completely unaffected by tournament changes
17. [ ] Performance test with 48 teams, 64+ matches

#### Step 4.6: Final Coverage & Cleanup

18. [ ] Run `coverage-unit.bat` - verify 100% line and branch coverage
19. [ ] Review all new code for UK English spelling
20. [ ] Review SQL for bracket/alias conventions
21. [ ] Review logging for correct format

**Deliverables:** Full tournament support ready for production use.

---

## Database Changes Summary

### New Columns

| Table | Column | Type | Nullable | Default | Purpose |
|-------|--------|------|----------|---------|---------|
| `Rounds` | `DisplayName` | nvarchar(200) | NO | '' | User-facing round name. "Gameweek 1" for leagues, "Group Stage - Matchday 1" for tournaments. |
| `Matches` | `ApiRoundName` | nvarchar(128) | YES | NULL | Original API round name for this match (e.g. "Group A - 1", "Quarter-finals") |

### Existing Columns Now Used

| Table | Column | Current State | Tournament Usage |
|-------|--------|---------------|-----------------|
| `Matches` | `CustomLockTimeUtc` | Exists, unused | Set on later-stage matches in combined rounds only |
| `Matches` | `PlaceholderHomeName` | Exists, unused | "Winner Group A" for TBC knockout matches |
| `Matches` | `PlaceholderAwayName` | Exists, unused | "Runner-up Group B" for TBC knockout matches |
| `Matches` | `HomeTeamId` | Nullable, always set | NULL for TBC matches |
| `Matches` | `AwayTeamId` | Nullable, always set | NULL for TBC matches |
| `Seasons` | `CompetitionType` | In DB only (default 0) | Added to domain model. 0=League, 1=Tournament |

### Tables NOT Being Created

| Originally Planned | Why Dropped |
|--------------------|-------------|
| `TournamentStages` lookup table | C# enum used only during sync. Not persisted. DisplayName on Round stores the result. |
| `TournamentGroups` table | Group info irrelevant to the prediction game. Not displayed anywhere. |

---

## Files to Create (New)

| File | Phase | Description |
|------|-------|-------------|
| `Domain/Common/Enumerations/CompetitionType.cs` | 1 | League = 0, Tournament = 1 |
| `Domain/Common/Enumerations/TournamentStage.cs` | 1 | Internal enum for sync grouping logic (Group, RoundOf32, RoundOf16, QuarterFinals, SemiFinals, ThirdPlace, Final) |
| SQL migration scripts | 1 | Schema changes for Rounds.DisplayName, Matches.ApiRoundName |
| `Web.Client/Components/Shared/CompetitionTypeBadge.razor` | 4 | Coloured badge component |

## Files to Modify (Existing)

| File | Phase | Changes |
|------|-------|---------|
| `Domain/Models/Season.cs` | 1 | Add CompetitionType, update factory/constructor |
| `Domain/Models/Round.cs` | 1 | Add DisplayName, update factory/constructor/UpdateDetails |
| `Domain/Models/Match.cs` | 1 | Add ApiRoundName, GetEffectiveDeadline, IsPredictionLocked, AreTeamsConfirmed, AssignTeams |
| `Domain/Services/PredictionDomainService.cs` | 3 | Per-match deadline logic for combined rounds, TBC match rejection |
| `Contracts/Admin/Seasons/BaseSeasonRequest.cs` | 1 | Add CompetitionType |
| `Contracts/Admin/Seasons/SeasonDto.cs` | 1 | Add CompetitionType |
| `Contracts/Dashboard/MatchPredictionDto.cs` | 3 | Add lock status, placeholders, AreTeamsConfirmed |
| `Contracts/Predictions/PredictionPageDto.cs` | 3 | Add IsTournament |
| `Validators/Admin/Seasons/BaseSeasonRequestValidator.cs` | 1 | Validate CompetitionType |
| `Application/Features/Admin/Seasons/Commands/CreateSeasonCommandHandler.cs` | 1 | Pass CompetitionType |
| `Application/Features/Admin/Seasons/Commands/UpdateSeasonCommandHandler.cs` | 1 | Pass CompetitionType |
| `Application/Features/Admin/Seasons/Commands/SyncSeasonWithApiCommandHandler.cs` | 2 | Tournament sync logic, auto-grouping |
| `Application/Features/Predictions/Queries/GetPredictionPageDataQueryHandler.cs` | 3 | LEFT JOIN teams, COALESCE, lock status |
| `Application/Features/Admin/Seasons/Queries/*` | 1 | Return CompetitionType |
| `Web.Client/Components/Pages/Predictions/Predictions.razor` | 3 | Lock badges, TBC display, DisplayName |
| `Web.Client/Components/Pages/Admin/Seasons/*` | 1 | CompetitionType selector |
| `docs/guides/database-schema.md` | 1 | Document new columns |

---

## Risk Register

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| API-Football 2026 World Cup data not available before May | Medium | Critical | Test with 2022 data; have manual entry as fallback |
| 2026 round name format differs from 2022 | Medium | Medium | Verify with API call before coding parser. Parser is easy to adjust. |
| Combined round locking logic introduces bugs in existing league flow | Low | High | Feature-flag via CompetitionType - leagues unchanged. CustomLockTimeUtc only set for tournaments. |
| 6-week timeline is tight for 4 phases | Medium | High | Phase 4 polish can be trimmed; core functionality prioritised |
| Existing tests break from Season/Round/Match model changes | Medium | Medium | Update constructors carefully; run tests after each change |

---

## Manual Steps Summary

These steps require the owner (not Claude) to perform:

| When | Action | Owner |
|------|--------|-------|
| Before Phase 1 | Provide API-Football API key in a Claude Desktop session for API verification | Owner |
| Phase 1 | Run SQL migration scripts against dev database | Owner |
| Phase 1 | Run SQL migration scripts against prod database | Owner |
| Phase 4 | Run integration tests against dev environment | Owner |
| Phase 4 | Final UAT / manual testing | Owner |
| Post-delivery | Monitor first tournament season for issues | Owner |

---

*Plan Version: 3.0*
*Original Plan: January 2026*
*Updated: March 2026 (v2 - codebase review)*
*Updated: March 2026 (v3 - technical decisions review, simplified architecture)*
