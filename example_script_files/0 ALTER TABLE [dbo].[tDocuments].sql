IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'tDocuments' AND COLUMN_NAME = 'isPrimary')
    BEGIN
    	ALTER TABLE [dbo].[tDocuments]
    		ADD isPrimary       BIT CONSTRAINT [DF_tDocuments_isPrimary] DEFAULT ((0)) NULL
    END