CREATE TABLE [dbo].[ProbationSetting] (
    [EmployeeId]  INT     NOT NULL,
    [ProcessId]   TINYINT NOT NULL,
    [EvaluatorId] INT     NOT NULL,
    [FromDate]    DATE    NOT NULL,
    [ToDate]      DATE    NOT NULL,
    CONSTRAINT [PK_ProbationSetting] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [ProcessId] ASC)
);

