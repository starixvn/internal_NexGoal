CREATE TABLE [dbo].[TargetTemplate] (
    [TargetTemplateId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [TargetTypeId]     TINYINT        NOT NULL,
    [IsActive]         BIT            NOT NULL,
    CONSTRAINT [PK_TargetTemplate] PRIMARY KEY CLUSTERED ([TargetTemplateId] ASC)
);

