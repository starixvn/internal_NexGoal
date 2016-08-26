CREATE TABLE [dbo].[EPAGroupDetail] (
    [EPAId]       INT            NOT NULL,
    [ProcessId]   INT            NOT NULL,
    [EvaluatorId] INT            NOT NULL,
    [KPIGroupId]  INT            NOT NULL,
    [Score]       NVARCHAR (MAX) NULL,
    [Weight]      FLOAT (53)     NULL,
    CONSTRAINT [PK__EPAGroupDetail] PRIMARY KEY CLUSTERED ([EPAId] ASC, [ProcessId] ASC, [EvaluatorId] ASC, [KPIGroupId] ASC)
);

