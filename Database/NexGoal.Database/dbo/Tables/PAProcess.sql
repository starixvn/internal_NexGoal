CREATE TABLE [dbo].[PAProcess] (
    [PARecordId]          INT     NOT NULL,
    [ProcessId]           TINYINT NOT NULL,
    [EvaluatorId]         INT     NOT NULL,
    [PerformanceRecordId] INT     NULL,
    [Priority]            TINYINT NULL,
    [StartDate]           DATE    NULL,
    [EndDate]             DATE    NULL,
    CONSTRAINT [PK_PAProcess] PRIMARY KEY CLUSTERED ([PARecordId] ASC, [ProcessId] ASC, [EvaluatorId] ASC)
);

