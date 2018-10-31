CREATE TABLE [dbo].[Meetings] (
    [Id]             INT          NOT NULL,
    [AuthorId]       INT          NOT NULL,
    [ProjectId]      INT          NOT NULL,
    [Subject]        VARCHAR (80) NOT NULL,
    [Description]    TEXT         NULL,
    [Date]           DATE         NULL,
    [StartHour]      TIME (7)     NULL,
    [EndHour]        TIME (7)     NULL,
    [EstimatedHours] INT          NULL,
    [Status]         INT          NULL,
    [UpdatedAt]      DATETIME     NOT NULL,
    [IssueId]        INT          NULL,
    [SynsDate]       DATETIME     NULL,
    CONSTRAINT [PK_Meetings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

