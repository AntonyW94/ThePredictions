-- Phase 2: Create TournamentRoundMappings table
-- Run against dev DB first, then prod after verification.
--
-- Stores the admin-configured tournament structure:
-- which tournament stages map to which prediction rounds.

CREATE TABLE [TournamentRoundMappings] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [SeasonId] INT NOT NULL,
    [RoundNumber] INT NOT NULL,
    [DisplayName] NVARCHAR(200) NOT NULL,
    [Stages] NVARCHAR(500) NOT NULL,
    [ExpectedMatchCount] INT NOT NULL,
    CONSTRAINT [PK_TournamentRoundMappings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TournamentRoundMappings_Seasons] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons]([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_TournamentRoundMappings_Season_Round] UNIQUE ([SeasonId], [RoundNumber])
);

-- Verify
SELECT * FROM [TournamentRoundMappings];
