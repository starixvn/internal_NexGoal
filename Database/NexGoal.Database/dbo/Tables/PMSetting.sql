CREATE TABLE [dbo].[PMSetting] (
    [ProjectManagerId] INT NOT NULL,
    [MemberId]         INT NOT NULL,
    CONSTRAINT [PK_PMSetting] PRIMARY KEY CLUSTERED ([ProjectManagerId] ASC, [MemberId] ASC)
);

