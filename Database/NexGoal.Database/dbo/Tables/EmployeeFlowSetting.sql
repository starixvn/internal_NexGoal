CREATE TABLE [dbo].[EmployeeFlowSetting] (
    [EmployeeId]  INT     NOT NULL,
    [ProcessId]   TINYINT NOT NULL,
    [EvaluatorId] INT     NOT NULL,
    [Priority]    TINYINT NULL,
    CONSTRAINT [PK_EmployeeFlowSetting] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [ProcessId] ASC, [EvaluatorId] ASC)
);



