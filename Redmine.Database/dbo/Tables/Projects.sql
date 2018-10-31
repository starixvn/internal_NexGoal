CREATE TABLE [dbo].[Projects] (
    [Id]          INT           NOT NULL,
    [Name]        VARCHAR (255) NOT NULL,
    [Description] TEXT          NULL,
    [ParentId]    INT           NULL,
    [UpdatedOn]   DATETIME      NULL,
    [Status]      INT           NOT NULL,
    [SynsDate]    DATETIME      NOT NULL,
    [CreatedOn]   DATETIME      NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([Id] ASC)
);

