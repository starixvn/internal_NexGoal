CREATE TABLE [dbo].[Members] (
    [Id]        INT      NOT NULL,
    [UserId]    INT      NOT NULL,
    [ProjectId] INT      NOT NULL,
    [CreatedOn] DATETIME NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY CLUSTERED ([Id] ASC)
);

