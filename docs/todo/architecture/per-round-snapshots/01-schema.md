# Task 1: Schema and Domain Models

**Parent Feature:** [Per-Round Historical Snapshots](./README.md)

## Status

**Not Started** | In Progress | Complete

## Goal

Add the two snapshot tables, the matching Domain entities, and the schema documentation. No write path or read path yet - just the storage layer.

## SQL

Both tables created in a single migration script. Manual migration for now (via `DatabaseInitialiser`); when DbUp lands this becomes a versioned migration.

```sql
CREATE TABLE [LeagueMemberRoundStats]
(
    [LeagueId]                 int             NOT NULL,
    [UserId]                   nvarchar(450)   NOT NULL,
    [RoundId]                  int             NOT NULL,

    [OverallRankAfterRound]    int             NOT NULL,
    [OverallPointsAfterRound]  decimal(10, 2)  NOT NULL,
    [MonthRankAfterRound]      int             NULL,
    [MonthPointsAfterRound]    decimal(10, 2)  NULL,

    [RoundRank]                int             NOT NULL,
    [RoundPointsRaw]           decimal(10, 2)  NOT NULL,
    [RoundPointsBoosted]       decimal(10, 2)  NOT NULL,

    [PredictionsSubmittedCount] int            NOT NULL,
    [ExactScoresCount]         int             NOT NULL,
    [CorrectResultsCount]      int             NOT NULL,
    [IncorrectCount]           int             NOT NULL,

    [BoostTypeUsed]            tinyint         NULL,
    [BoostPointsGained]        decimal(10, 2)  NULL,

    [TournamentStage]          tinyint         NULL,
    [CreatedAtUtc]             datetime2       NOT NULL,

    CONSTRAINT [PK_LeagueMemberRoundStats]
        PRIMARY KEY CLUSTERED ([LeagueId], [UserId], [RoundId]),
    CONSTRAINT [FK_LeagueMemberRoundStats_Leagues]
        FOREIGN KEY ([LeagueId]) REFERENCES [Leagues] ([Id]),
    CONSTRAINT [FK_LeagueMemberRoundStats_AspNetUsers]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_LeagueMemberRoundStats_Rounds]
        FOREIGN KEY ([RoundId]) REFERENCES [Rounds] ([Id])
);

CREATE NONCLUSTERED INDEX [IX_LeagueMemberRoundStats_UserId_LeagueId]
    ON [LeagueMemberRoundStats] ([UserId], [LeagueId])
    INCLUDE ([RoundId], [OverallRankAfterRound], [OverallPointsAfterRound]);

CREATE TABLE [LeagueRoundSnapshot]
(
    [LeagueId]                  int             NOT NULL,
    [RoundId]                   int             NOT NULL,

    [ParticipatingMemberCount]  int             NOT NULL,
    [TopScorerUserId]           nvarchar(450)   NULL,
    [TopScorerPoints]           decimal(10, 2)  NULL,
    [RoundAveragePoints]        decimal(10, 2)  NOT NULL,
    [RoundMaxPoints]            decimal(10, 2)  NOT NULL,
    [LeaderUserId]              nvarchar(450)   NOT NULL,
    [LeaderPoints]              decimal(10, 2)  NOT NULL,

    [ScoringRulesJson]          nvarchar(max)   NULL,
    [BoostRulesJson]            nvarchar(max)   NULL,
    [TournamentStage]           tinyint         NULL,
    [CreatedAtUtc]              datetime2       NOT NULL,

    CONSTRAINT [PK_LeagueRoundSnapshot]
        PRIMARY KEY CLUSTERED ([LeagueId], [RoundId]),
    CONSTRAINT [FK_LeagueRoundSnapshot_Leagues]
        FOREIGN KEY ([LeagueId]) REFERENCES [Leagues] ([Id]),
    CONSTRAINT [FK_LeagueRoundSnapshot_Rounds]
        FOREIGN KEY ([RoundId]) REFERENCES [Rounds] ([Id]),
    CONSTRAINT [FK_LeagueRoundSnapshot_TopScorerUser]
        FOREIGN KEY ([TopScorerUserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_LeagueRoundSnapshot_LeaderUser]
        FOREIGN KEY ([LeaderUserId]) REFERENCES [AspNetUsers] ([Id])
);
```

## Domain entities

`ThePredictions.Domain.Models.LeagueMemberRoundStats` and `LeagueRoundSnapshot`.

These are *value-style* entities - they're written once at round close and never mutated. Constructors take all required fields; no `Update*` methods. Per project rules:

- One public type per file.
- Use a static `Create(...)` factory for new instances (validates input).
- Add a parameterless `private` constructor for Dapper hydration; mark with `[ExcludeFromCodeCoverage]`.
- All `DateTime` properties suffixed `Utc`.

## `database-schema.md` update

Add both tables to `docs/guides/database-schema.md` in the same section as `LeagueMemberStats`. Document each column.

## `DatabaseRefresher.cs` update

Add both tables to `TableCopyOrder` in `tools/ThePredictions.DatabaseTools/DatabaseRefresher.cs`. Position them after their FK dependencies:

- `LeagueMemberRoundStats` after `Leagues`, `AspNetUsers`, `Rounds`.
- `LeagueRoundSnapshot` after the same set.

Neither table contains personal data so no `DataAnonymiser.cs` rules needed. No tokens so no `PersonalDataVerifier.cs` rules needed.

## Tests

- `LeagueMemberRoundStats.Create_ShouldThrow_WhenLeagueIdInvalid` etc. for guard clauses.
- `LeagueMemberRoundStats.Create_ShouldSetMonthFieldsToNull_WhenTournamentRound` (the only branching logic - if there is any; the entity itself probably accepts whatever it's given).
- `LeagueRoundSnapshot.Create_*` equivalents.

Domain coverage 100% line / branch as always.

## Verification

- [ ] Both tables exist in dev database.
- [ ] FKs are correctly defined and tested by attempting an insert that violates each.
- [ ] `database-schema.md` reflects the new tables.
- [ ] `dotnet test` passes.
- [ ] Domain coverage 100%.
- [ ] `DatabaseRefresher` runs end-to-end and includes the new tables.
