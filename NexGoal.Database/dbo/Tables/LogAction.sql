CREATE TABLE [dbo].[LogAction] (
    [ActionId]     SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [ActionTypeId] TINYINT        NOT NULL,
    [SPName]       VARCHAR (50)   NOT NULL,
    [Items]        NVARCHAR (MAX) NOT NULL,
    [SubItems]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_LogAction] PRIMARY KEY CLUSTERED ([ActionId] ASC)
);

