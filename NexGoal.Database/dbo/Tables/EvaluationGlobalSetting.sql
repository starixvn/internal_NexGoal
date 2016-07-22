CREATE TABLE [dbo].[EvaluationGlobalSetting] (
    [TargetTypeId]  INT NOT NULL,
    [ProcessId]     INT NOT NULL,
    [Duration]      INT NOT NULL,
    [NoticeAtBegin] BIT NOT NULL,
    [RemindDay]     INT NOT NULL
);

