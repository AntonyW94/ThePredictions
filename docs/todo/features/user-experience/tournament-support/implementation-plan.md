# Tournament Support - Implementation Plan

## Overview

This is the actionable implementation plan for adding tournament support to ThePredictions, targeting the FIFA World Cup 2026 (starting June 2026). This plan was derived from the [feature plan](README.md) and updated in March 2026 after a codebase review.

**Target Delivery:** May 2026
**Scope:** Pure tournament format (World Cup). Champions League hybrid format is deferred.
**Builder:** Claude (AI Assistant) via Claude Desktop app
**Database Migrations:** Manual SQL scripts run against production via SSMS

---

## Decisions Log

| Decision | Outcome | Date |
|----------|---------|------|
| Boosts in tournaments | Same as leagues - boosts work identically | Mar 2026 |
| Prize types for tournaments | Overall, Round, MostExactScores only. Monthly prizes hidden for tournament competitions. No new prize types. | Mar 2026 |
| Champions League / Hybrid support | Deferred - not in scope for May deadline | Mar 2026 |
| Database migrations | Manual SQL scripts via SSMS | Mar 2026 |

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
| No minimum match count per round | N/A | No code constraint exists (plan overstated this) |

### Gaps confirmed (work required)

| Gap | Impact |
|-----|--------|
| `CompetitionType` not in Season domain model, factory, constructor, DTOs, or admin UI | Cannot create/identify tournament seasons |
| `MatchPredictionDto` missing placeholder names, CustomLockTimeUtc, lock status | UI cannot display tournament match info |
| `GetPredictionPageDataQueryHandler` hard-joins Teams with no COALESCE fallback | Query fails for null team IDs |
| API sync handler has no tournament round parsing | Cannot sync World Cup fixtures |
| No `TournamentStage` or `TournamentGroup` tables/entities | No stage/group tracking |
| `Round` entity missing `TournamentStageId`, `ApiRoundNames`, `DisplayName` | No tournament round metadata |
| `Match` entity missing `TournamentGroupId`, `ApiRoundName` | No match-level tournament context |
| `PredictionDomainService` has no per-match deadline logic | All matches share round deadline |
| `DatabaseRefresher.cs` needs new tables in copy order | Dev DB refresh would miss tournament data |
| Prize filtering not competition-type-aware | Monthly prizes would appear for tournaments |

---

## Implementation Phases

### Phase 0: API Research & Data Modelling (Claude Desktop session)

> **This phase must be done first in a Claude Desktop session where API calls are possible.**

**Goal:** Call the football API with real World Cup data to verify assumptions and finalise DB/model structure.

**Tasks:**

1. [ ] Call API-Football endpoint for FIFA World Cup 2022 (League ID: 1, Season: 2022) to get fixture data
2. [ ] Analyse the actual `round` field values returned - verify they match the patterns assumed in the plan ("Group A - 1", "Quarter-finals", etc.)
3. [ ] Check if World Cup 2026 data is available yet (League ID: 1, Season: 2026) - if not, confirm 2022 structure is representative
4. [ ] Document the exact round name patterns, team placeholder handling, and any edge cases
5. [ ] Verify the `fixture.teams.home/away` structure for knockout matches - do they return null teams or placeholder names?
6. [ ] Confirm the API league IDs for competitions we want to support
7. [ ] Based on findings, confirm or adjust the proposed DB schema (TournamentStages seed data, TournamentGroups structure)
8. [ ] Document the custom lock time strategy - confirm 30 minutes before kick-off is appropriate

**Output:** Confirmed schema design and API integration approach documented in this file.

---

### Phase 1: Foundation - Database & Domain (Week 1-2)

**Goal:** Database schema updated, domain models extended, Season admin supports competition type.

#### Step 1.1: Database Schema Changes

**Manual step (owner runs SQL against dev DB first, then prod):**

SQL scripts to create:

1. [ ] `TournamentStages` lookup table with seed data (GROUP, ROUND_OF_32, ROUND_OF_16, QUARTER_FINALS, SEMI_FINALS, THIRD_PLACE, FINAL)
2. [ ] `TournamentGroups` table (SeasonId, GroupLetter, Name)
3. [ ] `ALTER [Rounds]` - add `TournamentStageId` (nullable FK), `ApiRoundNames` (nvarchar(500) nullable), `DisplayName` (nvarchar(100) nullable)
4. [ ] `ALTER [Matches]` - add `TournamentGroupId` (nullable FK), `ApiRoundName` (nvarchar(128) nullable)
5. [ ] Add `DEFAULT 0` constraint on `Seasons.CompetitionType` if not already present

**Claude builds:** SQL scripts ready to run. Owner executes manually.

#### Step 1.2: Update database-schema.md

6. [ ] Add `TournamentStages` table documentation
7. [ ] Add `TournamentGroups` table documentation
8. [ ] Update `Rounds` table with new columns
9. [ ] Update `Matches` table with new columns

#### Step 1.3: Update DatabaseRefresher.cs

10. [ ] Add `TournamentStages` to `TableCopyOrder` (before `Rounds`, after `Seasons`)
11. [ ] Add `TournamentGroups` to `TableCopyOrder` (after `TournamentStages`, before `Rounds`)
12. [ ] Confirm no anonymisation needed for these tables (no personal data)

#### Step 1.4: New Domain Enumerations

13. [ ] Create `CompetitionType.cs` enum (League = 0, Tournament = 1, Hybrid = 2)
14. [ ] Create `TournamentStageType.cs` enum (Group, Knockout)

#### Step 1.5: New Domain Entities

15. [ ] Create `TournamentStage.cs` entity (Id, Code, Name, DisplayOrder, StageType, MinMatchesForRound)
16. [ ] Create `TournamentGroup.cs` entity with factory method and constructor
17. [ ] Unit tests for `TournamentGroup.Create()` validation

#### Step 1.6: Update Season Domain Model

18. [ ] Add `CompetitionType` property to `Season`
19. [ ] Update `Season` constructor to accept `CompetitionType`
20. [ ] Update `Season.Create()` factory to accept `CompetitionType`
21. [ ] Update `Season.UpdateDetails()` to accept `CompetitionType`
22. [ ] Add helper: `IsTournament => CompetitionType == CompetitionType.Tournament`
23. [ ] Unit tests for Season with CompetitionType

#### Step 1.7: Update Round Domain Model

24. [ ] Add `TournamentStageId`, `ApiRoundNames`, `DisplayName` properties to `Round`
25. [ ] Update `Round` constructor to accept new properties
26. [ ] Update `Round.Create()` factory to optionally accept tournament properties
27. [ ] Add `GetApiRoundNamesList()` method
28. [ ] Unit tests for new Round properties and methods

#### Step 1.8: Update Match Domain Model

29. [ ] Add `TournamentGroupId`, `ApiRoundName` properties to `Match`
30. [ ] Update `Match` constructor to accept new properties
31. [ ] Add `Match.CreateWithPlaceholders()` factory method
32. [ ] Add `GetEffectiveDeadline(DateTime roundDeadline)` method
33. [ ] Add `IsPredictionLocked(DateTime utcNow, DateTime roundDeadline)` method
34. [ ] Add `AssignTeams(int homeTeamId, int awayTeamId)` method
35. [ ] Unit tests for all new Match methods

#### Step 1.9: Update Contracts & DTOs

36. [ ] Add `CompetitionType` to `BaseSeasonRequest` / `SeasonDto`
37. [ ] Update `CreateSeasonRequestValidator` for CompetitionType

#### Step 1.10: Update Application Layer

38. [ ] Create `ITournamentStageRepository` interface
39. [ ] Create `ITournamentGroupRepository` interface
40. [ ] Implement `TournamentStageRepository` in Infrastructure
41. [ ] Implement `TournamentGroupRepository` in Infrastructure
42. [ ] Update `CreateSeasonCommand` / handler to include CompetitionType
43. [ ] Update `UpdateSeasonCommand` / handler to include CompetitionType
44. [ ] Update `GetSeasonByIdQuery` / `FetchAllSeasonsQuery` to return CompetitionType
45. [ ] Register new repositories in DI

#### Step 1.11: Update Season Admin UI

46. [ ] Add CompetitionType dropdown to Season create/edit pages
47. [ ] Default to "League" for existing behaviour

#### Step 1.12: Run Tests & Coverage

48. [ ] Run unit tests - ensure 100% coverage maintained on Domain project
49. [ ] Run `coverage-unit.bat` to verify

**Deliverables:** Database updated, domain models extended, Season admin shows competition type.

---

### Phase 2: Per-Match Deadlines (Week 3)

**Goal:** Prediction submission respects per-match deadlines. UI shows lock status per match.

#### Step 2.1: Domain Service Update

1. [ ] Update `PredictionDomainService.SubmitPredictions` to support per-match deadline enforcement
2. [ ] Add parameter or season-based detection for when to use per-match deadlines
3. [ ] Unit tests for per-match deadline scenarios (locked match, unlocked match, mixed)

#### Step 2.2: Update Prediction Query

4. [ ] Update `GetPredictionPageDataQueryHandler` SQL to include `CustomLockTimeUtc`, `PlaceholderHomeName`, `PlaceholderAwayName`
5. [ ] Add COALESCE logic: `COALESCE(ht.[Name], m.[PlaceholderHomeName]) AS HomeTeamName`
6. [ ] Calculate `IsLocked` and `EffectiveDeadlineUtc` in the query/mapping

#### Step 2.3: Update DTOs

7. [ ] Add to `MatchPredictionDto`: `CustomLockTimeUtc`, `EffectiveDeadlineUtc`, `IsLocked`, `PlaceholderHomeName`, `PlaceholderAwayName`, `GroupName`, `StageName`
8. [ ] Add to `PredictionPageDto`: `HasPerMatchDeadlines`, `TournamentStageName`

#### Step 2.4: Update Prediction Submission

9. [ ] Update `SubmitPredictionsCommand` handler to detect tournament seasons and enforce per-match deadlines
10. [ ] Ensure locked matches are rejected, unlocked matches are accepted (in the same round)

#### Step 2.5: Update Prediction UI

11. [ ] Add per-match lock indicator badge to prediction cards
12. [ ] Show individual countdown/deadline per match when `HasPerMatchDeadlines` is true
13. [ ] Disable prediction inputs for locked matches
14. [ ] Display placeholder team names when real team names are null

#### Step 2.6: Tests & Coverage

15. [ ] Unit tests for domain service deadline logic
16. [ ] Verify 100% coverage

**Deliverables:** Per-match deadlines enforced, UI shows lock status per match.

---

### Phase 3: Tournament Round Creation & API Sync (Week 3-4)

**Goal:** World Cup seasons can be synced from the API. Admin can create tournament prediction rounds.

#### Step 3.1: API Sync Updates

1. [ ] Add round name parsing logic to `SyncSeasonWithApiCommandHandler` (or new tournament-specific sync handler)
2. [ ] Parse "Group A - 1" → stage=GROUP, group=A, matchday=1
3. [ ] Parse "Quarter-finals", "Semi-finals", etc. → appropriate stage codes
4. [ ] Auto-create `TournamentGroup` records during sync
5. [ ] Set `CustomLockTimeUtc` on all tournament matches (30 min before kick-off)
6. [ ] Set `ApiRoundName` on individual matches
7. [ ] Set `TournamentGroupId` on group stage matches
8. [ ] Handle placeholder teams for future knockout matches (if API provides them)

#### Step 3.2: Tournament Round Command

9. [ ] Create `CreateTournamentRoundCommand` (SeasonId, RoundNumber, DisplayName, ApiRoundNames list, TournamentStageId, OverrideDeadlineUtc)
10. [ ] Handler groups matches from specified API rounds into one prediction round
11. [ ] Auto-calculates deadline as earliest match time minus 30 minutes (unless overridden)

#### Step 3.3: Admin UI for Tournament Rounds

12. [ ] Tournament-aware round creation page/modal
13. [ ] Show available API round names as multi-select checkboxes
14. [ ] Preview matches that will be included
15. [ ] Allow setting display name and optional deadline override

#### Step 3.4: Tests

16. [ ] Unit tests for round name parsing (all patterns)
17. [ ] Unit tests for tournament round creation
18. [ ] Verify 100% coverage

**Deliverables:** Tournament sync working, admin can create grouped prediction rounds.

---

### Phase 4: Placeholder Teams & Team Assignment (Week 4-5)

**Goal:** Knockout matches show "Winner Group A vs Runner-up Group B". Teams assigned when determined.

#### Step 4.1: Placeholder Match Creation

1. [ ] Ensure `Match.CreateWithPlaceholders()` is used during sync for knockout matches
2. [ ] Verify placeholder names display correctly in prediction page
3. [ ] Verify prediction cards handle null team logos gracefully

#### Step 4.2: Team Assignment

4. [ ] Create `AssignTournamentTeamsCommand` for when group results are finalised
5. [ ] Handler calls `Match.AssignTeams()` and clears placeholder names
6. [ ] API sync should auto-assign teams when the API returns real team IDs for previously-placeholder matches

#### Step 4.3: Prediction Rules for Placeholders

7. [ ] Decide: allow predictions on matches with placeholder teams? (Recommended: YES - users should be able to predict scores before teams are known)
8. [ ] If match teams change after predictions are submitted, predictions remain valid (score prediction, not team prediction)

#### Step 4.4: Tests

9. [ ] Unit tests for AssignTeams method
10. [ ] Unit tests for CreateWithPlaceholders
11. [ ] Verify 100% coverage

**Deliverables:** Placeholder teams display correctly, teams can be assigned when known.

---

### Phase 5: UI Polish, Prizes & Testing (Week 5-6)

**Goal:** Full tournament prediction flow working end-to-end. Prizes correct. Mobile-friendly.

#### Step 5.1: Grouped Match Display

1. [ ] Group matches by tournament group/stage on prediction page
2. [ ] Add group headers ("Group A", "Group B") and stage headers ("Quarter-finals")
3. [ ] Sort: by group letter, then by match time

#### Step 5.2: Dashboard Updates

4. [ ] Show tournament stage badges on round cards
5. [ ] Display group letters for group stage matches
6. [ ] Show "TBD" for placeholder teams in upcoming rounds

#### Step 5.3: Prize Filtering

7. [ ] Filter out Monthly prize type when league's season has `CompetitionType = Tournament`
8. [ ] Update prize configuration UI to hide Monthly option for tournament leagues
9. [ ] Update prize processing logic to skip Monthly for tournaments
10. [ ] Verify Overall, Round, and MostExactScores work correctly with tournament round structure

#### Step 5.4: Mobile Responsiveness

11. [ ] Test tournament UI on mobile viewports
12. [ ] Ensure per-match deadline badges don't break layout
13. [ ] Ensure grouped match display works on small screens

#### Step 5.5: Integration Testing

14. [ ] End-to-end test: create tournament season → sync → create rounds → submit predictions → enter results → view leaderboard
15. [ ] Test with World Cup 2022 API data (League ID: 1, Season: 2022)
16. [ ] Test admin workflow for managing tournament rounds
17. [ ] Test mixed locked/unlocked matches in same round
18. [ ] Performance test with 48 teams, 64+ matches

#### Step 5.6: Final Coverage & Cleanup

19. [ ] Run `coverage-unit.bat` - verify 100% line and branch coverage
20. [ ] Review all new code for UK English spelling
21. [ ] Review SQL for bracket/alias conventions
22. [ ] Review logging for correct format

**Deliverables:** Full tournament support ready for production use.

---

## Manual Steps Summary

These steps require the owner (not Claude) to perform:

| When | Action | Owner |
|------|--------|-------|
| Phase 0 | Provide API-Football API key for Claude to call endpoints | Owner |
| Phase 1 | Run SQL migration scripts against dev database | Owner |
| Phase 1 | Run SQL migration scripts against prod database | Owner |
| Phase 5 | Run integration tests against dev environment | Owner |
| Phase 5 | Final UAT / manual testing | Owner |
| Post-delivery | Monitor first tournament season for issues | Owner |

---

## Files to Create (New)

| File | Phase | Description |
|------|-------|-------------|
| `Domain/Common/Enumerations/CompetitionType.cs` | 1 | League, Tournament, Hybrid enum |
| `Domain/Common/Enumerations/TournamentStageType.cs` | 1 | Group, Knockout enum |
| `Domain/Models/TournamentStage.cs` | 1 | Stage lookup entity |
| `Domain/Models/TournamentGroup.cs` | 1 | Group entity with factory |
| `Application/Repositories/ITournamentStageRepository.cs` | 1 | Stage repository interface |
| `Application/Repositories/ITournamentGroupRepository.cs` | 1 | Group repository interface |
| `Infrastructure/Repositories/TournamentStageRepository.cs` | 1 | Stage repository implementation |
| `Infrastructure/Repositories/TournamentGroupRepository.cs` | 1 | Group repository implementation |
| `Application/Features/Admin/Rounds/Commands/CreateTournamentRoundCommand.cs` | 3 | Grouped round creation |
| `Application/Features/Admin/Rounds/Commands/CreateTournamentRoundCommandHandler.cs` | 3 | Handler |
| `Application/Features/Admin/Matches/Commands/AssignTournamentTeamsCommand.cs` | 4 | Team assignment |
| `Contracts/Dashboard/MatchGroupDto.cs` | 5 | Grouped matches DTO |
| `Web.Client/Components/Pages/Predictions/MatchLockBadge.razor` | 2 | Lock indicator component |
| SQL migration scripts (multiple) | 1 | Schema changes |

## Files to Modify (Existing)

| File | Phase | Changes |
|------|-------|---------|
| `Domain/Models/Season.cs` | 1 | Add CompetitionType, update factory/constructor |
| `Domain/Models/Round.cs` | 1 | Add tournament properties, update factory/constructor |
| `Domain/Models/Match.cs` | 1 | Add tournament properties, new factory, deadline methods |
| `Domain/Services/PredictionDomainService.cs` | 2 | Per-match deadline logic |
| `Contracts/Admin/Seasons/BaseSeasonRequest.cs` | 1 | Add CompetitionType |
| `Contracts/Admin/Seasons/SeasonDto.cs` | 1 | Add CompetitionType |
| `Contracts/Dashboard/MatchPredictionDto.cs` | 2 | Add lock status, placeholders |
| `Contracts/Predictions/PredictionPageDto.cs` | 2 | Add tournament properties |
| `Validators/Admin/Seasons/BaseSeasonRequestValidator.cs` | 1 | Validate CompetitionType |
| `Application/Features/Admin/Seasons/Commands/CreateSeasonCommandHandler.cs` | 1 | Pass CompetitionType |
| `Application/Features/Admin/Seasons/Commands/UpdateSeasonCommandHandler.cs` | 1 | Pass CompetitionType |
| `Application/Features/Admin/Seasons/Commands/SyncSeasonWithApiCommandHandler.cs` | 3 | Tournament sync logic |
| `Application/Features/Predictions/Queries/GetPredictionPageDataQueryHandler.cs` | 2 | Tournament query fields |
| `Application/Features/Admin/Seasons/Queries/*` | 1 | Return CompetitionType |
| `Infrastructure/DependencyInjection.cs` | 1 | Register new repositories |
| `Web.Client/Components/Pages/Predictions/Predictions.razor` | 2 | Per-match UI |
| `Web.Client/Components/Pages/Admin/Seasons/*` | 1 | CompetitionType selector |
| `tools/ThePredictions.DatabaseTools/DatabaseRefresher.cs` | 1 | Add new tables to copy order |
| `docs/guides/database-schema.md` | 1 | Document new tables and columns |

---

## Risk Register

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| API-Football 2026 World Cup data not available before May | Medium | Critical | Test with 2022 data; have manual entry as fallback |
| Per-match deadline logic introduces bugs in existing league flow | Low | High | Feature-flag via CompetitionType - leagues unchanged |
| 6-week timeline is tight for 5 phases | Medium | High | Phase 5 polish can be trimmed; core functionality prioritised |
| Existing tests break from Season/Round/Match model changes | Medium | Medium | Update constructors carefully; run tests after each change |

---

*Plan Version: 2.0*
*Original Plan: January 2026*
*Updated: March 2026*
*Review Session: Claude Code codebase analysis*
