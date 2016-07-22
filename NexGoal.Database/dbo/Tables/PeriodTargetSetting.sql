CREATE TABLE [dbo].[PeriodTargetSetting] (
    [PeriodId]         INT      NOT NULL,
    [EmployeeId]       INT      NOT NULL,
    [TargetTemplateId] INT      NOT NULL,
    [ModifiedBy]       INT      NULL,
    [ModifiedDate]     DATETIME NULL
);

