CREATE TABLE [dbo].[ProbationEvaluationDetail] (
    [EmployeeId]     INT           NOT NULL,
    [KPIId]          INT           NOT NULL,
    [SelfPoint]      VARCHAR (MAX) NULL,
    [EvaluatorPoint] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_ProbationEvaluationDetail] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [KPIId] ASC)
);

