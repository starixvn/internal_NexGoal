CREATE TABLE [dbo].[TargetTemplateKPI] (
    [TargetTemplateId] INT      NOT NULL,
    [KPIGroupId]       INT      NOT NULL,
    [KPIId]            INT      NOT NULL,
    [DisplayOrder]     SMALLINT NOT NULL,
    CONSTRAINT [PK_TargetTemplateKPI] PRIMARY KEY CLUSTERED ([TargetTemplateId] ASC, [KPIGroupId] ASC, [KPIId] ASC)
);

