CREATE TABLE [dbo].[CategoryMessage] (
    [CategoryId]   INT            NOT NULL,
    [Name]         NVARCHAR (100) NULL,
    [IsActive]     BIT            NULL,
    [AffectedDate] DATETIME       NULL,
    [IsRegister]   BIT            NULL,
    CONSTRAINT [PK_CategoryMessage] PRIMARY KEY CLUSTERED ([CategoryId] ASC)
);

