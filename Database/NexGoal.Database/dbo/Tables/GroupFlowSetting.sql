CREATE TABLE [dbo].[GroupFlowSetting] (
    [GroupId]     INT     NOT NULL,
    [ProcessId]   TINYINT NOT NULL,
    [EvaluatorId] INT     NOT NULL,
    [Priority]    TINYINT NULL,
    CONSTRAINT [PK_GroupFlowSetting] PRIMARY KEY CLUSTERED ([GroupId] ASC, [ProcessId] ASC, [EvaluatorId] ASC)
);



