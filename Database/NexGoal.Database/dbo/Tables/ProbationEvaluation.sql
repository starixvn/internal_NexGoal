CREATE TABLE [dbo].[ProbationEvaluation] (
    [EmployeeId]             INT            NOT NULL,
    [IsQualified]            BIT            NULL,
    [CreatedDate]            DATETIME       NOT NULL,
    [SelfComment]            NVARCHAR (MAX) NULL,
    [EvaluatorComment]       NVARCHAR (MAX) NULL,
    [ReviewerComment]        NVARCHAR (MAX) NULL,
    [SelfScore]              VARCHAR (MAX)  NULL,
    [EvaluatorScore]         VARCHAR (MAX)  NULL,
    [IsQualifiedByEvaluator] BIT            NULL,
    PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);

