CREATE TABLE [dbo].[PAFocusProcessSetting] (
    [PeriodId]  INT NOT NULL,
    [ProcessId] INT NOT NULL,
    [Duration]  INT NULL,
    CONSTRAINT [pk_PAFocusProcessSetting] PRIMARY KEY CLUSTERED ([PeriodId] ASC, [ProcessId] ASC)
);

