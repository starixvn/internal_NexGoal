CREATE TABLE [dbo].[LogActionType] (
    [ActionTypeId] TINYINT       IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_LogActionType] PRIMARY KEY CLUSTERED ([ActionTypeId] ASC)
);

