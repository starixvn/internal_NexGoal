CREATE TABLE [dbo].[PAFocusProcess] (
    [PAFocusId]   INT      NOT NULL,
    [EvaluatorId] INT      NOT NULL,
    [ProcessId]   INT      NOT NULL,
    [StartDate]   DATETIME NULL,
    [EndDate]     DATETIME NULL,
    CONSTRAINT [pk_PAFocusProcess] PRIMARY KEY CLUSTERED ([PAFocusId] ASC, [EvaluatorId] ASC, [ProcessId] ASC)
);

