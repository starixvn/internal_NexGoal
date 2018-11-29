CREATE TABLE [dbo].[FlowTemplate] (
    [FlowTemplateId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (200) NOT NULL,
    [TargetTypeId]   TINYINT        NOT NULL,
    [Description]    NVARCHAR (500) NULL,
    [IsActive]       BIT            NULL,
    CONSTRAINT [PK_FlowTemplate] PRIMARY KEY CLUSTERED ([FlowTemplateId] ASC)
);

