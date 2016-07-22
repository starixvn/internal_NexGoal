CREATE TABLE [dbo].[TargetTemplateDepartment] (
    [TargetTemplateId] INT      NOT NULL,
    [DepartmentId]     INT      NOT NULL,
    [IsGroup]          BIT      NOT NULL,
    [ModifiedBy]       INT      NULL,
    [ModifiedDate]     DATETIME NULL
);

