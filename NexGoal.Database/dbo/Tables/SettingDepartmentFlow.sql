CREATE TABLE [dbo].[SettingDepartmentFlow] (
    [DepartmentId]   INT      NOT NULL,
    [ProcessId]      INT      NOT NULL,
    [EvaluatorId]    INT      NOT NULL,
    [IsGroup]        BIT      NOT NULL,
    [FlowTemplateId] INT      NOT NULL,
    [Priority]       TINYINT  NULL,
    [Duration]       INT      NULL,
    [HasSendMail]    BIT      NULL,
    [RemindDay]      INT      NULL,
    [ModifiedBy]     INT      NULL,
    [ModifiedDate]   DATETIME NULL
);

