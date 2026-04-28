-- Add legal consent timestamp columns to AspNetUsers
-- Run on both dev and prod
-- Captures WHEN the user consented during registration (GDPR proof of consent)

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[AspNetUsers]') AND name = 'Over18ConfirmedAtUtc')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Over18ConfirmedAtUtc] datetime2 NULL;
    PRINT 'Added Over18ConfirmedAtUtc column to AspNetUsers';
END
ELSE
    PRINT 'Over18ConfirmedAtUtc column already exists';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[AspNetUsers]') AND name = 'TermsAcceptedAtUtc')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [TermsAcceptedAtUtc] datetime2 NULL;
    PRINT 'Added TermsAcceptedAtUtc column to AspNetUsers';
END
ELSE
    PRINT 'TermsAcceptedAtUtc column already exists';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[AspNetUsers]') AND name = 'MarketingOptInAtUtc')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [MarketingOptInAtUtc] datetime2 NULL;
    PRINT 'Added MarketingOptInAtUtc column to AspNetUsers';
END
ELSE
    PRINT 'MarketingOptInAtUtc column already exists';
GO
