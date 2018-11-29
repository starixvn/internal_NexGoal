CREATE TABLE [dbo].[EmployeeFlowSetting] (
    [FlowTemplateId] SMALLINT NOT NULL,
    [EmployeeId]     INT      NOT NULL,
    [ProcessId]      TINYINT  NOT NULL,
    [EvaluatorId]    INT      NOT NULL,
    [Priority]       TINYINT  NULL,
    CONSTRAINT [PK_EmployeeFlowSetting] PRIMARY KEY CLUSTERED ([FlowTemplateId] ASC, [EmployeeId] ASC, [ProcessId] ASC, [EvaluatorId] ASC)
);

