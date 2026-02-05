-- =============================================
-- Bitcoin Tracking Application - Database Schema
-- =============================================
-- This script creates the database and tables needed for the Bitcoin Tracking application.
-- Entity Framework Code First approach was used, this script is for manual DB creation.
-- 
-- Author: [Your Name]
-- Date: 2026-02-05
-- Version: 1.0
-- =============================================

USE master;
GO

-- =============================================
-- 1. CREATE DATABASE (if not exists)
-- =============================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BitcoinTrackingDb')
BEGIN
    CREATE DATABASE BitcoinTrackingDb;
    PRINT 'Database BitcoinTrackingDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database BitcoinTrackingDb already exists.';
END
GO

USE BitcoinTrackingDb;
GO

-- =============================================
-- 2. CREATE TABLE: BitcoinRecords
-- =============================================
-- This table stores historical Bitcoin price records
-- Each record contains BTC/EUR price, EUR/CZK exchange rate,
-- calculated BTC/CZK price, timestamp, and optional note.
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BitcoinRecords]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BitcoinRecords] (
        -- Primary Key
        [Id] INT IDENTITY(1,1) NOT NULL,
        
        -- Bitcoin price in EUR (from CoinDesk API)
        [PriceBtcEur] DECIMAL(18, 4) NOT NULL,
        
        -- EUR to CZK exchange rate (from ČNB API)
        [ExchangeRateEurCzk] DECIMAL(18, 4) NOT NULL,
        
        -- Calculated Bitcoin price in CZK (PriceBtcEur * ExchangeRateEurCzk)
        [PriceBtcCzk] DECIMAL(18, 4) NOT NULL,
        
        -- Timestamp when the record was created (UTC)
        [Timestamp] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        
        -- Optional user note (max 500 characters)
        -- UNIQUE constraint added to prevent duplicate notes
        [Note] NVARCHAR(500) NOT NULL,
        
        -- Primary Key Constraint
        CONSTRAINT [PK_BitcoinRecords] PRIMARY KEY CLUSTERED ([Id] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            ON [PRIMARY]
    ) ON [PRIMARY];
    
    PRINT 'Table BitcoinRecords created successfully.';
END
ELSE
BEGIN
    PRINT 'Table BitcoinRecords already exists.';
END
GO

-- =============================================
-- 3. CREATE UNIQUE CONSTRAINT ON NOTE
-- =============================================
-- Prevents duplicate notes from being saved
-- This constraint was added to ensure data quality
-- =============================================

IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'UQ_BitcoinRecords_Note' 
    AND object_id = OBJECT_ID('BitcoinRecords')
)
BEGIN
    ALTER TABLE [dbo].[BitcoinRecords]
    ADD CONSTRAINT [UQ_BitcoinRecords_Note] UNIQUE ([Note]);
    
    PRINT 'Unique constraint UQ_BitcoinRecords_Note created successfully.';
END
ELSE
BEGIN
    PRINT 'Unique constraint UQ_BitcoinRecords_Note already exists.';
END
GO

-- =============================================
-- 4. CREATE INDEXES FOR PERFORMANCE
-- =============================================
-- Index on Timestamp for faster queries sorted by date
-- =============================================

IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'IX_BitcoinRecords_Timestamp' 
    AND object_id = OBJECT_ID('BitcoinRecords')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_BitcoinRecords_Timestamp]
    ON [dbo].[BitcoinRecords] ([Timestamp] DESC)
    INCLUDE ([PriceBtcEur], [ExchangeRateEurCzk], [PriceBtcCzk], [Note]);
    
    PRINT 'Index IX_BitcoinRecords_Timestamp created successfully.';
END
ELSE
BEGIN
    PRINT 'Index IX_BitcoinRecords_Timestamp already exists.';
END
GO

-- =============================================
-- 5. INSERT SAMPLE DATA (Optional - for testing)
-- =============================================
-- Uncomment the following lines to insert sample records
-- =============================================

/*
INSERT INTO [dbo].[BitcoinRecords] 
    ([PriceBtcEur], [ExchangeRateEurCzk], [PriceBtcCzk], [Timestamp], [Note])
VALUES
    (66689.10, 24.3000, 1620545.13, '2026-02-03 00:22:00', N'Test záznam z API'),
    (66700.00, 24.3000, 1620810.00, '2026-02-02 23:17:00', N'Test z Live Data'),
    (85432.50, 25.1500, 2148629.38, '2026-02-02 12:05:00', N'Poznámka: Postman'),
    (85432.50, 25.1500, 2148629.38, '2026-02-01 22:42:00', N'Test záznam z API'),
    (85432.50, 25.1500, 2148629.38, '2026-02-01 22:20:00', N'Test záznam z API');

PRINT 'Sample data inserted successfully.';
*/

-- =============================================
-- 6. VERIFICATION QUERY
-- =============================================
-- Check if table was created correctly
-- =============================================

SELECT 
    'BitcoinRecords' AS TableName,
    COUNT(*) AS RecordCount,
    MIN([Timestamp]) AS OldestRecord,
    MAX([Timestamp]) AS NewestRecord
FROM [dbo].[BitcoinRecords];
GO

-- =============================================
-- 7. GRANT PERMISSIONS (Optional)
-- =============================================
-- Grant permissions to application user if needed
-- =============================================

/*
-- Replace 'YourAppUser' with your actual database user
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[BitcoinRecords] TO [YourAppUser];
GO
*/

PRINT 'Database schema creation completed successfully!';
PRINT '================================================';
PRINT 'Next steps:';
PRINT '1. Update connection string in appsettings.json';
PRINT '2. Run the application';
PRINT '3. Test API endpoints';
PRINT '================================================';
GO




