-- FIFA World Cup 2026 Knockout Match Dates
-- Source: https://www.fifa.com/en/tournaments/mens/worldcup/canadamexicousa2026/articles/match-schedule-fixtures-results-teams-stadiums
-- Times are placeholder (20:00 UTC) — will be updated by API sync when exact times are available
-- Season ID: 4

-- Verify round IDs before running
SELECT r.[Id], r.[RoundNumber], r.[DisplayName],
    COUNT(m.[Id]) AS MatchCount
FROM [Rounds] r
LEFT JOIN [Matches] m ON r.[Id] = m.[RoundId]
WHERE r.[SeasonId] = 4 AND r.[RoundNumber] >= 4
GROUP BY r.[Id], r.[RoundNumber], r.[DisplayName]
ORDER BY r.[RoundNumber];

-- =====================================================
-- Round of 32 (16 matches) — Matches 73-88
-- Round ID: 52 (verify above before running)
-- =====================================================
;WITH R32Numbered AS (
    SELECT
        m.[Id],
        ROW_NUMBER() OVER (ORDER BY m.[Id]) AS MatchNum
    FROM [Matches] m
    WHERE m.[RoundId] = 52
)
UPDATE m SET m.[MatchDateTimeUtc] = dates.MatchDateTimeUtc
FROM [Matches] m
INNER JOIN R32Numbered n ON m.[Id] = n.[Id]
INNER JOIN (VALUES
    -- Match 73: Sun 28 June
    (1,  '2026-06-28T20:00:00'),
    -- Match 74-76: Mon 29 June
    (2,  '2026-06-29T18:00:00'),
    (3,  '2026-06-29T20:00:00'),
    (4,  '2026-06-29T22:00:00'),
    -- Match 77-79: Tue 30 June
    (5,  '2026-06-30T18:00:00'),
    (6,  '2026-06-30T20:00:00'),
    (7,  '2026-06-30T22:00:00'),
    -- Match 80-82: Wed 1 July
    (8,  '2026-07-01T18:00:00'),
    (9,  '2026-07-01T20:00:00'),
    (10, '2026-07-01T22:00:00'),
    -- Match 83-85: Thu 2 July
    (11, '2026-07-02T18:00:00'),
    (12, '2026-07-02T20:00:00'),
    (13, '2026-07-02T22:00:00'),
    -- Match 86-88: Fri 3 July
    (14, '2026-07-03T18:00:00'),
    (15, '2026-07-03T20:00:00'),
    (16, '2026-07-03T22:00:00')
) AS dates(MatchNum, MatchDateTimeUtc)
ON n.MatchNum = dates.MatchNum;

-- =====================================================
-- Round of 16 (8 matches) — Matches 89-96
-- Round ID: 53 (verify above before running)
-- =====================================================
;WITH R16Numbered AS (
    SELECT
        m.[Id],
        ROW_NUMBER() OVER (ORDER BY m.[Id]) AS MatchNum
    FROM [Matches] m
    WHERE m.[RoundId] = 53
)
UPDATE m SET m.[MatchDateTimeUtc] = dates.MatchDateTimeUtc
FROM [Matches] m
INNER JOIN R16Numbered n ON m.[Id] = n.[Id]
INNER JOIN (VALUES
    -- Match 89-90: Sat 4 July
    (1, '2026-07-04T18:00:00'),
    (2, '2026-07-04T21:00:00'),
    -- Match 91-92: Sun 5 July
    (3, '2026-07-05T18:00:00'),
    (4, '2026-07-05T21:00:00'),
    -- Match 93-94: Mon 6 July
    (5, '2026-07-06T18:00:00'),
    (6, '2026-07-06T21:00:00'),
    -- Match 95-96: Tue 7 July
    (7, '2026-07-07T18:00:00'),
    (8, '2026-07-07T21:00:00')
) AS dates(MatchNum, MatchDateTimeUtc)
ON n.MatchNum = dates.MatchNum;

-- =====================================================
-- Quarter-finals (4 matches) — Matches 97-100
-- Round ID: 54 (verify above before running)
-- =====================================================
;WITH QFNumbered AS (
    SELECT
        m.[Id],
        ROW_NUMBER() OVER (ORDER BY m.[Id]) AS MatchNum
    FROM [Matches] m
    WHERE m.[RoundId] = 54
)
UPDATE m SET m.[MatchDateTimeUtc] = dates.MatchDateTimeUtc
FROM [Matches] m
INNER JOIN QFNumbered n ON m.[Id] = n.[Id]
INNER JOIN (VALUES
    -- Match 97: Thu 9 July
    (1, '2026-07-09T20:00:00'),
    -- Match 98: Fri 10 July
    (2, '2026-07-10T20:00:00'),
    -- Match 99-100: Sat 11 July
    (3, '2026-07-11T18:00:00'),
    (4, '2026-07-11T21:00:00')
) AS dates(MatchNum, MatchDateTimeUtc)
ON n.MatchNum = dates.MatchNum;

-- =====================================================
-- Finals round (4 matches: 2 SF + 1 Third Place + 1 Final)
-- Round ID: 55 (verify above before running)
-- Placeholder creation order: SF1, SF2, Third Place, Final
-- =====================================================
;WITH FinalsNumbered AS (
    SELECT
        m.[Id],
        ROW_NUMBER() OVER (ORDER BY m.[Id]) AS MatchNum
    FROM [Matches] m
    WHERE m.[RoundId] = 55
)
UPDATE m SET m.[MatchDateTimeUtc] = dates.MatchDateTimeUtc
FROM [Matches] m
INNER JOIN FinalsNumbered n ON m.[Id] = n.[Id]
INNER JOIN (VALUES
    -- Match 101 (SF1): Tue 14 July
    (1, '2026-07-14T20:00:00'),
    -- Match 102 (SF2): Wed 15 July
    (2, '2026-07-15T20:00:00'),
    -- Match 103 (Third Place): Sat 18 July
    (3, '2026-07-18T20:00:00'),
    -- Match 104 (Final): Sun 19 July
    (4, '2026-07-19T20:00:00')
) AS dates(MatchNum, MatchDateTimeUtc)
ON n.MatchNum = dates.MatchNum;

-- =====================================================
-- Verify results
-- =====================================================
SELECT
    r.[DisplayName],
    m.[Id] AS MatchId,
    m.[MatchDateTimeUtc],
    m.[PlaceholderHomeName],
    m.[PlaceholderAwayName]
FROM [Rounds] r
INNER JOIN [Matches] m ON r.[Id] = m.[RoundId]
WHERE r.[SeasonId] = 4 AND r.[RoundNumber] >= 4
ORDER BY r.[RoundNumber], m.[Id];
