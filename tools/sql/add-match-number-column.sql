-- Add MatchNumber column to Matches table
-- Run on both dev and prod

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Matches]') AND name = 'MatchNumber')
BEGIN
    ALTER TABLE [Matches] ADD [MatchNumber] int NULL;
    PRINT 'Added MatchNumber column to Matches table';
END
ELSE
    PRINT 'MatchNumber column already exists';
GO

-- =====================================================
-- Assign match numbers for existing World Cup season (Season 4)
-- Numbers 1-104 matching FIFA match numbering
-- =====================================================

;WITH AllMatches AS (
    SELECT
        m.[Id],
        r.[RoundNumber],
        ROW_NUMBER() OVER (ORDER BY r.[RoundNumber], m.[Id]) AS MatchNumber
    FROM [Matches] m
    INNER JOIN [Rounds] r ON m.[RoundId] = r.[Id]
    WHERE r.[SeasonId] = 4
)
UPDATE m SET m.[MatchNumber] = am.MatchNumber
FROM [Matches] m
INNER JOIN AllMatches am ON m.[Id] = am.[Id];

-- Verify
SELECT
    r.[DisplayName],
    m.[Id],
    m.[MatchNumber],
    m.[PlaceholderHomeName],
    m.[PlaceholderAwayName]
FROM [Rounds] r
INNER JOIN [Matches] m ON r.[Id] = m.[RoundId]
WHERE r.[SeasonId] = 4
ORDER BY m.[MatchNumber];
GO

-- =====================================================
-- Update knockout placeholder names to "Winner Match X" format
-- Based on FIFA World Cup 2026 bracket structure
-- =====================================================

-- Round of 16 (matches 89-96): Winners of R32 matches
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 74', [PlaceholderAwayName] = 'Winner Match 77' WHERE [MatchNumber] = 89;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 73', [PlaceholderAwayName] = 'Winner Match 75' WHERE [MatchNumber] = 90;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 76', [PlaceholderAwayName] = 'Winner Match 78' WHERE [MatchNumber] = 91;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 79', [PlaceholderAwayName] = 'Winner Match 80' WHERE [MatchNumber] = 92;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 83', [PlaceholderAwayName] = 'Winner Match 84' WHERE [MatchNumber] = 93;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 81', [PlaceholderAwayName] = 'Winner Match 82' WHERE [MatchNumber] = 94;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 86', [PlaceholderAwayName] = 'Winner Match 88' WHERE [MatchNumber] = 95;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 85', [PlaceholderAwayName] = 'Winner Match 87' WHERE [MatchNumber] = 96;

-- Quarter-finals (matches 97-100): Winners of R16 matches
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 89', [PlaceholderAwayName] = 'Winner Match 90' WHERE [MatchNumber] = 97;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 93', [PlaceholderAwayName] = 'Winner Match 94' WHERE [MatchNumber] = 98;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 91', [PlaceholderAwayName] = 'Winner Match 92' WHERE [MatchNumber] = 99;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 95', [PlaceholderAwayName] = 'Winner Match 96' WHERE [MatchNumber] = 100;

-- Semi-finals (matches 101-102): Winners of QF matches
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 97', [PlaceholderAwayName] = 'Winner Match 98' WHERE [MatchNumber] = 101;
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 99', [PlaceholderAwayName] = 'Winner Match 100' WHERE [MatchNumber] = 102;

-- Third Place (match 103): Losers of SF matches
UPDATE [Matches] SET [PlaceholderHomeName] = 'Loser Match 101', [PlaceholderAwayName] = 'Loser Match 102' WHERE [MatchNumber] = 103;

-- Final (match 104): Winners of SF matches
UPDATE [Matches] SET [PlaceholderHomeName] = 'Winner Match 101', [PlaceholderAwayName] = 'Winner Match 102' WHERE [MatchNumber] = 104;

PRINT 'Updated knockout placeholder names with FIFA bracket references';
GO
