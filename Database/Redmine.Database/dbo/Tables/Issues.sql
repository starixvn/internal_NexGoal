CREATE TABLE [dbo].[Issues] (
    [Id]             INT           NOT NULL,
    [TrackerId]      INT           NOT NULL,
    [ProjectId]      INT           NOT NULL,
    [Subject]        VARCHAR (255) NOT NULL,
    [Description]    TEXT          NULL,
    [DueDate]        DATE          NULL,
    [CategoryId]     INT           NULL,
    [StatusId]       INT           NOT NULL,
    [AssignedId]     INT           NULL,
    [PriorityId]     INT           NOT NULL,
    [AuthorId]       INT           NOT NULL,
    [CreatedOn]      DATETIME      NULL,
    [UpdatedOn]      DATETIME      NULL,
    [StartDate]      DATE          NULL,
    [EstimatedHours] FLOAT (53)    NULL,
    [ParentId]       INT           NULL,
    [RootId]         INT           NULL,
    [ClosedOn]       DATETIME      NULL,
    [SynsDate]       DATETIME      NOT NULL,
    CONSTRAINT [PK_Issues] PRIMARY KEY CLUSTERED ([Id] ASC)
);

