-- Phase 0: Insert World Cup 2026 Teams
-- Run against dev DB first, then prod after verification.
-- Generated from API-Football data (league=1, season=2026) on 2 April 2026.
--
-- Pre-check: Ensure no duplicates by ApiTeamId
-- SELECT [ApiTeamId], [Name] FROM [Teams] WHERE [ApiTeamId] IN (1,2,3,5,6,7,8,9,10,11,12,13,15,16,17,20,22,23,25,26,27,28,31,32,770,775,777,1090,1108,1113,1118,1501,1504,1508,1531,1532,1533,1548,1567,1568,1569,2380,2382,2384,2386,4673,5529,5530);

BEGIN TRANSACTION;

INSERT INTO [Teams] ([Name], [ShortName], [Abbreviation], [LogoUrl], [ApiTeamId])
VALUES
    -- Group Stage teams (48 teams, ordered by API team ID)
    (N'Belgium', N'Belgium', N'BEL', N'https://media.api-sports.io/football/teams/1.png', 1),
    (N'France', N'France', N'FRA', N'https://media.api-sports.io/football/teams/2.png', 2),
    (N'Croatia', N'Croatia', N'CRO', N'https://media.api-sports.io/football/teams/3.png', 3),
    (N'Sweden', N'Sweden', N'SWE', N'https://media.api-sports.io/football/teams/5.png', 5),
    (N'Brazil', N'Brazil', N'BRA', N'https://media.api-sports.io/football/teams/6.png', 6),
    (N'Uruguay', N'Uruguay', N'URU', N'https://media.api-sports.io/football/teams/7.png', 7),
    (N'Colombia', N'Colombia', N'COL', N'https://media.api-sports.io/football/teams/8.png', 8),
    (N'Spain', N'Spain', N'SPA', N'https://media.api-sports.io/football/teams/9.png', 9),
    (N'England', N'England', N'ENG', N'https://media.api-sports.io/football/teams/10.png', 10),
    (N'Panama', N'Panama', N'PAN', N'https://media.api-sports.io/football/teams/11.png', 11),
    (N'Japan', N'Japan', N'JAP', N'https://media.api-sports.io/football/teams/12.png', 12),
    (N'Senegal', N'Senegal', N'SEN', N'https://media.api-sports.io/football/teams/13.png', 13),
    (N'Switzerland', N'Switzerland', N'SWI', N'https://media.api-sports.io/football/teams/15.png', 15),
    (N'Mexico', N'Mexico', N'MEX', N'https://media.api-sports.io/football/teams/16.png', 16),
    (N'South Korea', N'South Korea', N'KOR', N'https://media.api-sports.io/football/teams/17.png', 17),
    (N'Australia', N'Australia', N'AUS', N'https://media.api-sports.io/football/teams/20.png', 20),
    (N'Iran', N'Iran', N'IRN', N'https://media.api-sports.io/football/teams/22.png', 22),
    (N'Saudi Arabia', N'Saudi Arabia', N'SAU', N'https://media.api-sports.io/football/teams/23.png', 23),
    (N'Germany', N'Germany', N'GER', N'https://media.api-sports.io/football/teams/25.png', 25),
    (N'Argentina', N'Argentina', N'ARG', N'https://media.api-sports.io/football/teams/26.png', 26),
    (N'Portugal', N'Portugal', N'POR', N'https://media.api-sports.io/football/teams/27.png', 27),
    (N'Tunisia', N'Tunisia', N'TUN', N'https://media.api-sports.io/football/teams/28.png', 28),
    (N'Morocco', N'Morocco', N'MOR', N'https://media.api-sports.io/football/teams/31.png', 31),
    (N'Egypt', N'Egypt', N'EGY', N'https://media.api-sports.io/football/teams/32.png', 32),
    (N'Czech Republic', N'Czech Republic', N'CZE', N'https://media.api-sports.io/football/teams/770.png', 770),
    (N'Austria', N'Austria', N'AUT', N'https://media.api-sports.io/football/teams/775.png', 775),
    (N'T' + NCHAR(0x00FC) + N'rkiye', N'T' + NCHAR(0x00FC) + N'rkiye', N'TUR', N'https://media.api-sports.io/football/teams/777.png', 777),
    (N'Norway', N'Norway', N'NOR', N'https://media.api-sports.io/football/teams/1090.png', 1090),
    (N'Scotland', N'Scotland', N'SCO', N'https://media.api-sports.io/football/teams/1108.png', 1108),
    (N'Bosnia & Herzegovina', N'Bosnia', N'BOS', N'https://media.api-sports.io/football/teams/1113.png', 1113),
    (N'Netherlands', N'Netherlands', N'NET', N'https://media.api-sports.io/football/teams/1118.png', 1118),
    (N'Ivory Coast', N'Ivory Coast', N'IVO', N'https://media.api-sports.io/football/teams/1501.png', 1501),
    (N'Ghana', N'Ghana', N'GHA', N'https://media.api-sports.io/football/teams/1504.png', 1504),
    (N'Congo DR', N'Congo DR', N'CON', N'https://media.api-sports.io/football/teams/1508.png', 1508),
    (N'South Africa', N'South Africa', N'RSA', N'https://media.api-sports.io/football/teams/1531.png', 1531),
    (N'Algeria', N'Algeria', N'ALG', N'https://media.api-sports.io/football/teams/1532.png', 1532),
    (N'Cape Verde Islands', N'Cape Verde', N'CAP', N'https://media.api-sports.io/football/teams/1533.png', 1533),
    (N'Jordan', N'Jordan', N'JOR', N'https://media.api-sports.io/football/teams/1548.png', 1548),
    (N'Iraq', N'Iraq', N'IRQ', N'https://media.api-sports.io/football/teams/1567.png', 1567),
    (N'Uzbekistan', N'Uzbekistan', N'UZB', N'https://media.api-sports.io/football/teams/1568.png', 1568),
    (N'Qatar', N'Qatar', N'QAT', N'https://media.api-sports.io/football/teams/1569.png', 1569),
    (N'Paraguay', N'Paraguay', N'PAR', N'https://media.api-sports.io/football/teams/2380.png', 2380),
    (N'Ecuador', N'Ecuador', N'ECU', N'https://media.api-sports.io/football/teams/2382.png', 2382),
    (N'USA', N'USA', N'USA', N'https://media.api-sports.io/football/teams/2384.png', 2384),
    (N'Haiti', N'Haiti', N'HAI', N'https://media.api-sports.io/football/teams/2386.png', 2386),
    (N'New Zealand', N'New Zealand', N'ZEA', N'https://media.api-sports.io/football/teams/4673.png', 4673),
    (N'Canada', N'Canada', N'CAN', N'https://media.api-sports.io/football/teams/5529.png', 5529),
    (N'Cura' + NCHAR(0x00E7) + N'ao', N'Cura' + NCHAR(0x00E7) + N'ao', N'CUR', N'https://media.api-sports.io/football/teams/5530.png', 5530);

-- Verify: should return 48 rows
SELECT COUNT(*) AS [NewTeamCount] FROM [Teams] WHERE [ApiTeamId] IN (1,2,3,5,6,7,8,9,10,11,12,13,15,16,17,20,22,23,25,26,27,28,31,32,770,775,777,1090,1108,1113,1118,1501,1504,1508,1531,1532,1533,1548,1567,1568,1569,2380,2382,2384,2386,4673,5529,5530);

COMMIT TRANSACTION;
