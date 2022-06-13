CREATE TABLE [dbo].[TargetTemplateKPIGroup] (
    [TargetTemplateId] INT      NOT NULL,
    [KPIGroupId]       INT      NOT NULL,
    [DisplayOrder]     SMALLINT NOT NULL,
    CONSTRAINT [PK_TargetTemplateKPIGroup] PRIMARY KEY CLUSTERED ([TargetTemplateId] ASC, [KPIGroupId] ASC)
);

