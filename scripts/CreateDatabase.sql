IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [BitcoinRecords] (
    [Id] int NOT NULL IDENTITY,
    [Timestamp] datetime2 NOT NULL DEFAULT (GETDATE()),
    [PriceBtcEur] decimal(18,2) NOT NULL,
    [ExchangeRateEurCzk] decimal(18,4) NOT NULL,
    [PriceBtcCzk] decimal(18,2) NOT NULL,
    [Note] nvarchar(500) NOT NULL DEFAULT N'',
    CONSTRAINT [PK_BitcoinRecords] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_BitcoinRecords_Timestamp] ON [BitcoinRecords] ([Timestamp]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260131002139_InitialCreate', N'8.0.11');
GO

COMMIT;
GO

