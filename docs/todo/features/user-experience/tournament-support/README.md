# Tournament Support Feature Plan

## Overview

This document outlines a comprehensive plan to extend ThePredictions to support tournament-format competitions (e.g., FIFA World Cup 2026) alongside the existing league format (e.g., Premier League).

**Target Delivery:** May 2026 (for FIFA World Cup 2026 starting June 2026)

---

## Table of Contents

1. [Current State Analysis](#1-current-state-analysis)
2. [Tournament Requirements](#2-tournament-requirements)
3. [API-Football Integration Research](#3-api-football-integration-research)
4. [Architecture Design](#4-architecture-design)
5. [Database Schema Changes](#5-database-schema-changes)
6. [Domain Model Changes](#6-domain-model-changes)
7. [Application Layer Changes](#7-application-layer-changes)
8. [UI/UX Changes](#8-uiux-changes)
9. [Implementation Phases](#9-implementation-phases)
10. [Risk Analysis](#10-risk-analysis)
11. [Testing Strategy](#11-testing-strategy)

---

## 1. Current State Analysis

### Existing Architecture Strengths

The current codebase already has several columns/features that support tournament implementation:

| Feature | Status | Notes |
|---------|--------|-------|
| `Match.CustomLockTimeUtc` | Column exists, unused | Per-match deadline support ready |
| `Match.PlaceholderHomeName/AwayName` | Column exists, unused | "Winner Group A" style placeholders ready |
| `Match.HomeTeamId/AwayTeamId` nullable | Implemented | Supports TBD teams |
| `Seasons.CompetitionType` | In DB schema, not in domain | Needs domain model update |
| Prize strategies | Extensible pattern | Can add tournament-specific strategies |
| Configurable scoring | Per league | `PointsForExactScore`, `PointsForCorrectResult` |

### Current Limitations

| Limitation | Impact | Solution |
|------------|--------|----------|
| No `DisplayName` on Round | High | Add column, use for all display (leagues = "Gameweek N", tournaments = stage name) |
| No `ApiRoundName` on Match | Medium | Add column so each match tracks its API origin |
| `CompetitionType` not in domain | High | Add to Season entity, factory, constructor, DTOs |
| API sync assumes league format | High | Branch sync logic by CompetitionType |
| No auto-grouping for tournament rounds | High | Implement grouping rules in sync handler |

---

## 2. Tournament Requirements

### 2.1 Core Requirements

1. **Automatic round grouping** - System groups API rounds into prediction rounds based on rules (no admin involvement)
2. **Minimum 4 matches per prediction round** - Hard business rule; stages with fewer matches are combined
3. **Standard round deadlines** - 30 min before first match in round (same as leagues)
4. **Combined round locking** - When stages are combined, later-stage matches get `CustomLockTimeUtc` after preceding stage completes
5. **TBC teams** - Matches exist with null team IDs until preceding stage finishes; users cannot predict until both teams known
6. **Placeholder team names** - "Winner Group A" / "Runner-up Group B" shown for TBC matches
7. **90-minute scoring** - Extra time and penalties do not count

### 2.2 Competition Types

| Competition Type | Format | Example |
|------------------|--------|---------|
| **League** | Linear rounds, same teams | Premier League |
| **Tournament** | Groups then Knockout | FIFA World Cup |

Note: Champions League hybrid format is deferred (not in scope for May 2026).

### 2.3 FIFA World Cup 2026 Structure

- **48 teams** in 12 groups of 4
- **Group Stage:** 3 matchdays, ~24 matches per matchday = 3 prediction rounds
- **Round of 32:** 16 matches = 1 prediction round
- **Round of 16:** 8 matches = 1 prediction round
- **Quarter-finals:** 4 matches = 1 prediction round
- **Semi-finals + Final + 3rd Place:** 4 matches combined = 1 prediction round

**Total: ~7 prediction rounds** for the entire tournament.

### 2.4 Auto-Grouping Rules

| Stage | Matches | Grouping Rule | Prediction Round Name |
|-------|---------|---------------|----------------------|
| Group Stage (per matchday) | ~24 | All groups' matchday N = one round | "Group Stage - Matchday {N}" |
| Round of 32 | 16 | Own round (>= 4) | "Round of 32" |
| Round of 16 | 8 | Own round (>= 4) | "Round of 16" |
| Quarter-finals | 4 | Own round (>= 4) | "Quarter-finals" |
| Semi-finals | 2 | Combined with next stages (<4) | "Semi-finals, Final & Third Place Playoff" |
| 3rd Place | 1 | Combined (above) | (included above) |
| Final | 1 | Combined (above) | (included above) |

### 2.5 Combined Round Locking Flow

For the last round (Semi-finals + Final + 3rd Place Playoff):

1. Round deadline = 30 min before first semi-final
2. Users submit semi-final predictions before deadline
3. Semi-finals are played; Final + 3rd place show "TBC" teams
4. API sync runs: assigns real teams to Final + 3rd place matches
5. System sets `CustomLockTimeUtc` on Final + 3rd place = 30 min before first of those matches
6. Users return to the predict screen: semi-final predictions shown read-only, Final + 3rd place now editable
7. Users submit Final + 3rd place predictions before their `CustomLockTimeUtc`

### 2.6 Prize Types

| Prize Type | Available for Tournaments? |
|------------|--------------------------|
| Overall | Yes |
| Round | Yes |
| Most Exact Scores | Yes |
| Monthly | No (hidden for tournaments) |

---

## 3. API-Football Integration Research

### 3.1 Round Name Patterns

| Stage | API Round Name Pattern | Example |
|-------|----------------------|---------|
| League | `Regular Season - {N}` | `Regular Season - 16` |
| Group Stage | `Group {Letter} - {Matchday}` | `Group A - 1`, `Group B - 2` |
| Round of 32 | `Round of 32` | Single value |
| Round of 16 | `Round of 16` | Single value |
| Quarter-finals | `Quarter-finals` | Single value |
| Semi-finals | `Semi-finals` | Single value |
| Third-place | `3rd Place Final` | Single value |
| Final | `Final` | Single value |

> **IMPORTANT:** These patterns are based on 2022 World Cup data. The 2026 data MUST be verified via API call before coding the parser. See [implementation-plan.md](implementation-plan.md) "BEFORE YOU START" section.

### 3.2 Key Integration Points

1. **Round Name Parsing** - Parse API round name to determine TournamentStage enum value
2. **Auto-Grouping** - Group parsed rounds into prediction rounds based on rules
3. **DisplayName Generation** - Generate user-friendly round names from grouped stages
4. **Team Detection** - Detect null team IDs for TBC knockout matches

### 3.3 API Coverage

- FIFA World Cup: League ID `1`
- Season parameter: `2026` (or `2022` for testing)

Sources:
- [API-Football Documentation](https://www.api-football.com/documentation-v3)
- [API-Sports Documentation](https://api-sports.io/documentation/football/v3)

---

## 4. Architecture Design

### 4.1 Conceptual Model

```
Season (CompetitionType = Tournament)
  └── Rounds (auto-created by sync, DisplayName set)
        ├── Round 1: "Group Stage - Matchday 1" (deadline: 30min before first match)
        │     └── 24 Matches (ApiRoundName: "Group A - 1", "Group B - 1", ..., "Group L - 1")
        ├── Round 2: "Group Stage - Matchday 2"
        ├── Round 3: "Group Stage - Matchday 3"
        ├── Round 4: "Round of 32"
        ├── Round 5: "Round of 16"
        ├── Round 6: "Quarter-finals"
        └── Round 7: "Semi-finals, Final & Third Place Playoff" (combined, <4 matches per stage)
              ├── Match: Semi-final 1 (locks at round deadline)
              ├── Match: Semi-final 2 (locks at round deadline)
              ├── Match: 3rd Place Playoff (CustomLockTimeUtc, TBC teams until semis done)
              └── Match: Final (CustomLockTimeUtc, TBC teams until semis done)
```

### 4.2 Key Design Decisions

| # | Decision | Rationale |
|---|----------|-----------|
| 1 | **Reuse Round entity** with `DisplayName` | All downstream code (predictions, scoring, leaderboards, prizes) operates on Rounds. No parallel entity needed. |
| 2 | **No TournamentStages table** | C# enum during sync only. DisplayName stores the result. Avoids table, repository, DI, refresh tool. |
| 3 | **No TournamentGroups table** | Group info irrelevant to predictions. Not displayed. |
| 4 | **No ApiRoundNames (plural) on Round** | Each Match has its own ApiRoundName. Round doesn't need to redundantly track the aggregate. |
| 5 | **DisplayName for ALL rounds** | Leagues: "Gameweek {N}". Tournaments: stage name. Single source of truth for display. RoundNumber becomes internal only. |
| 6 | **Standard round deadlines** | Same as leagues. CustomLockTimeUtc only for combined round edge cases. |
| 7 | **Fully automatic grouping** | No admin involvement. Rules-based during sync. |

---

## 5. Database Schema Changes

### 5.1 New Columns

```sql
-- Rounds: Add DisplayName for all rounds
ALTER TABLE [Rounds] ADD [DisplayName] NVARCHAR(200) NOT NULL DEFAULT '';
-- Backfill: UPDATE [Rounds] SET [DisplayName] = 'Gameweek ' + CAST([RoundNumber] AS NVARCHAR)

-- Matches: Track original API round name
ALTER TABLE [Matches] ADD [ApiRoundName] NVARCHAR(128) NULL;
```

### 5.2 Existing Columns Now Used (no schema change needed)

| Column | New Usage |
|--------|-----------|
| `Seasons.CompetitionType` | Added to domain model (already in DB with default 0) |
| `Matches.CustomLockTimeUtc` | Set on later-stage matches in combined rounds |
| `Matches.PlaceholderHomeName` | "Winner Group A" for TBC matches |
| `Matches.PlaceholderAwayName` | "Runner-up Group B" for TBC matches |
| `Matches.HomeTeamId` (nullable) | NULL for TBC matches |
| `Matches.AwayTeamId` (nullable) | NULL for TBC matches |

### 5.3 Tables NOT Being Created

| Originally Discussed | Why Dropped |
|---------------------|-------------|
| `TournamentStages` | C# enum only. Not persisted. |
| `TournamentGroups` | Irrelevant to predictions. Not displayed. |

---

## 6. Domain Model Changes

### 6.1 New Enumerations

```csharp
// CompetitionType.cs - Persisted on Season
public enum CompetitionType
{
    League = 0,
    Tournament = 1
}

// TournamentStage.cs - Used only during sync, NOT persisted
public enum TournamentStage
{
    Group,
    RoundOf32,
    RoundOf16,
    QuarterFinals,
    SemiFinals,
    ThirdPlace,
    Final
}
```

### 6.2 Season Changes

- Add `CompetitionType` property
- Update constructor, factory, UpdateDetails
- Add `IsTournament` helper

### 6.3 Round Changes

- Add `DisplayName` (string, non-nullable)
- Update constructor, factory, UpdateDetails

### 6.4 Match Changes

- Add `ApiRoundName` (string?, nullable)
- Add `GetEffectiveDeadline(DateTime roundDeadline)` method
- Add `IsPredictionLocked(DateTime utcNow, DateTime roundDeadline)` method
- Add `AreTeamsConfirmed` property
- Add `AssignTeams(int homeTeamId, int awayTeamId)` method

---

## 7. Application Layer Changes

### 7.1 Sync Handler Changes

The `SyncSeasonWithApiCommandHandler` branches based on `CompetitionType`:
- **League:** Existing logic unchanged
- **Tournament:** New path with round name parsing, auto-grouping, and combined round deadline logic

No new repositories needed. No new commands for tournament round creation (it's all automatic in sync).

### 7.2 Prediction Submission Changes

`PredictionDomainService`:
- Reject predictions for matches where `AreTeamsConfirmed` is false
- Check `CustomLockTimeUtc` on individual matches (for combined rounds)
- Standard round deadline check for all other matches

### 7.3 Query Changes

`GetPredictionPageDataQueryHandler`:
- LEFT JOIN on Teams (not INNER) for null team IDs
- COALESCE for team names with placeholder fallback
- Return lock status, effective deadline, and AreTeamsConfirmed per match

---

## 8. UI/UX Changes

### 8.1 Competition Type Badge

Coloured badge/chip on season cards, round cards, and league cards:
- **League:** Blue badge with list icon - "League"
- **Tournament:** Amber/gold badge with trophy icon - "Tournament"

### 8.2 Prediction Page

- Use `DisplayName` for round title (not "Gameweek N")
- Show lock indicator badge on matches with CustomLockTimeUtc
- Disable inputs for locked matches (show as read-only)
- Disable inputs for TBC matches (show "TBC" for teams, greyed out)
- Show placeholder team names ("Winner Group A") for TBC matches

### 8.3 Dashboard

- Show DisplayName on round cards
- Show competition type badges
- Show "TBC" for placeholder teams

---

## 9. Implementation Phases

See [implementation-plan.md](implementation-plan.md) for the detailed, actionable implementation plan with specific tasks and checklists.

| Phase | Duration | Summary |
|-------|----------|---------|
| API Verification | Pre-Phase 1 | Call API to verify round name patterns and 2026 data availability |
| Phase 1: Foundation | Week 1-2 | DB schema, domain models, Season admin, DisplayName |
| Phase 2: Tournament Sync | Week 3-4 | Round name parser, auto-grouping, sync handler |
| Phase 3: Prediction Logic & UI | Week 4-5 | Per-match locking, TBC handling, prediction UI |
| Phase 4: Polish & Prizes | Week 5-6 | Competition badges, prize filtering, testing |

---

## 10. Risk Analysis

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| API 2026 data not available | Medium | Critical | Test with 2022 data; manual entry fallback |
| 2026 round names differ from 2022 | Medium | Medium | Verify via API call before coding parser |
| Combined round locking complexity | Medium | High | Isolated to tournament path; leagues unaffected |
| Timeline tight (6 weeks) | Medium | High | Phase 4 polish can be trimmed |

---

## 11. Testing Strategy

### Unit Tests

- TournamentStage round name parsing (all patterns)
- Auto-grouping logic (various stage sizes, minimum 4 rule)
- Combined round deadline logic
- `Match.GetEffectiveDeadline`, `IsPredictionLocked`, `AreTeamsConfirmed`, `AssignTeams`
- `PredictionDomainService` with TBC matches and combined round deadlines
- Season with CompetitionType
- Round with DisplayName

### Integration Tests

- Full tournament sync flow with test API data
- Predict -> lock -> assign teams -> predict remaining
- Leaderboard calculation for tournament leagues
- Prize processing skips Monthly for tournaments

### Test Data

Use FIFA World Cup 2022 (League ID: 1, Season: 2022) data from API-Football for testing.

---

*Document Version: 3.0*
*Created: January 2026*
*Updated: March 2026 (simplified architecture, removed unnecessary tables)*
