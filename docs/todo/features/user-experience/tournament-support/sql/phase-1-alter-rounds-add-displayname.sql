-- Phase 1: Add DisplayName column to Rounds table
-- Run against dev DB first, then prod after verification.
--
-- DisplayName is the user-facing round name:
--   League rounds: "Gameweek 1", "Gameweek 2", etc.
--   Tournament rounds: "Group Stage - Matchday 1", "Quarter-finals", etc.

ALTER TABLE [Rounds] ADD [DisplayName] NVARCHAR(200) NOT NULL DEFAULT '';

-- Backfill existing rounds with "Gameweek {N}" format
UPDATE [Rounds] SET [DisplayName] = N'Gameweek ' + CAST([RoundNumber] AS NVARCHAR(10));

-- Verify
SELECT [Id], [RoundNumber], [DisplayName] FROM [Rounds] ORDER BY [SeasonId], [RoundNumber];
