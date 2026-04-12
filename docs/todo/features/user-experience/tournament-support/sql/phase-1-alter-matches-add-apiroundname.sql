-- Phase 1: Add ApiRoundName column to Matches table
-- Run against dev DB first, then prod after verification.
--
-- ApiRoundName stores the original API round name for each match.
-- For leagues: e.g. "Regular Season - 1"
-- For tournaments: e.g. "Group Stage - 1", "Quarter-finals"
-- Nullable because existing matches won't have this data.

ALTER TABLE [Matches] ADD [ApiRoundName] NVARCHAR(128) NULL;
