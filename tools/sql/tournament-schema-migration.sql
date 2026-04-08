-- =====================================================
-- Tournament Support Schema Migration
-- Run on prod (and dev if not already applied)
-- Safe to re-run — all statements check IF NOT EXISTS
-- =====================================================

-- 1. Seasons: Add CompetitionType column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Seasons]') AND name = 'CompetitionType')
BEGIN
    ALTER TABLE [Seasons] ADD [CompetitionType] int NOT NULL DEFAULT 0;
    PRINT 'Added CompetitionType to Seasons';
END
ELSE PRINT 'CompetitionType already exists on Seasons';
GO

-- 2. Rounds: Add DisplayName column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Rounds]') AND name = 'DisplayName')
BEGIN
    ALTER TABLE [Rounds] ADD [DisplayName] nvarchar(200) NOT NULL DEFAULT '';
    PRINT 'Added DisplayName to Rounds';

    -- Backfill existing rounds with "Gameweek N" format
    UPDATE [Rounds] SET [DisplayName] = 'Gameweek ' + CAST([RoundNumber] AS nvarchar(10)) WHERE [DisplayName] = '';
    PRINT 'Backfilled DisplayName on existing rounds';
END
ELSE PRINT 'DisplayName already exists on Rounds';
GO

-- 3. Matches: Add tournament columns
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Matches]') AND name = 'CustomLockTimeUtc')
BEGIN
    ALTER TABLE [Matches] ADD [CustomLockTimeUtc] datetime2 NULL;
    PRINT 'Added CustomLockTimeUtc to Matches';
END
ELSE PRINT 'CustomLockTimeUtc already exists on Matches';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Matches]') AND name = 'PlaceholderHomeName')
BEGIN
    ALTER TABLE [Matches] ADD [PlaceholderHomeName] nvarchar(100) NULL;
    PRINT 'Added PlaceholderHomeName to Matches';
END
ELSE PRINT 'PlaceholderHomeName already exists on Matches';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Matches]') AND name = 'PlaceholderAwayName')
BEGIN
    ALTER TABLE [Matches] ADD [PlaceholderAwayName] nvarchar(100) NULL;
    PRINT 'Added PlaceholderAwayName to Matches';
END
ELSE PRINT 'PlaceholderAwayName already exists on Matches';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Matches]') AND name = 'ApiRoundName')
BEGIN
    ALTER TABLE [Matches] ADD [ApiRoundName] nvarchar(128) NULL;
    PRINT 'Added ApiRoundName to Matches';
END
ELSE PRINT 'ApiRoundName already exists on Matches';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Matches]') AND name = 'MatchNumber')
BEGIN
    ALTER TABLE [Matches] ADD [MatchNumber] int NULL;
    PRINT 'Added MatchNumber to Matches';
END
ELSE PRINT 'MatchNumber already exists on Matches';
GO

-- 4. TournamentRoundMappings table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('[TournamentRoundMappings]') AND type = 'U')
BEGIN
    CREATE TABLE [TournamentRoundMappings] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [SeasonId] int NOT NULL,
        [RoundNumber] int NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [Stages] nvarchar(500) NOT NULL,
        [ExpectedMatchCount] int NOT NULL,
        CONSTRAINT [PK_TournamentRoundMappings] PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_TournamentRoundMappings_SeasonId_RoundNumber] UNIQUE ([SeasonId], [RoundNumber]),
        CONSTRAINT [FK_TournamentRoundMappings_Seasons] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons]([Id]) ON DELETE CASCADE
    );
    PRINT 'Created TournamentRoundMappings table';
END
ELSE PRINT 'TournamentRoundMappings table already exists';
GO

PRINT '';
PRINT '=== Migration complete ===';
PRINT 'You can now create tournament seasons via the admin UI.';
GO
