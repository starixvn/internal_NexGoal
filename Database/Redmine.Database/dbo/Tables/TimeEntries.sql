CREATE TABLE [dbo].[TimeEntries] (
    [Id]         INT        NOT NULL,
    [ProjectId]  INT        NOT NULL,
    [UserId]     INT        NOT NULL,
    [IssueId]    INT        NULL,
    [Hours]      FLOAT (53) NOT NULL,
    [ActivityId] INT        NOT NULL,
    [SpentOn]    DATE       NOT NULL,
    [CreatedOn]  DATETIME   NOT NULL,
    [UpdatedOn]  DATETIME   NOT NULL,
    [MeetingId]  INT        NULL,
    [SynsDate]   DATETIME   NOT NULL,
    CONSTRAINT [PK_TimeEntries] PRIMARY KEY CLUSTERED ([Id] ASC)
);

