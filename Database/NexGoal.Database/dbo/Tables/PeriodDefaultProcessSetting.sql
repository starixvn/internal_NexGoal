CREATE TABLE [dbo].[PeriodDefaultProcessSetting] (
    [TargetTypeId]  INT NOT NULL,
    [ProcessId]     INT NOT NULL,
    [Duration]      INT NOT NULL,
    [NoticeAtBegin] BIT NOT NULL,
    [RemindDay]     INT NOT NULL,
    [FocusDay]      INT NULL,
    CONSTRAINT [PK_PeriodDefaultProcessSetting] PRIMARY KEY CLUSTERED ([TargetTypeId] ASC, [ProcessId] ASC) ON [PRIMARY]
) ON [PRIMARY];

