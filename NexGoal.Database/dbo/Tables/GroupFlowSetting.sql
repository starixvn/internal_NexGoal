CREATE TABLE [dbo].[GroupFlowSetting] (
    [FlowTemplateId] SMALLINT NOT NULL,
    [GroupId]        INT      NOT NULL,
    [ProcessId]      TINYINT  NOT NULL,
    [EvaluatorId]    INT      NOT NULL,
    [Priority]       TINYINT  NULL,
    CONSTRAINT [PK_GroupFlowSetting] PRIMARY KEY CLUSTERED ([FlowTemplateId] ASC, [GroupId] ASC, [ProcessId] ASC, [EvaluatorId] ASC)
);

