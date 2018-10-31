CREATE TABLE [dbo].[Users] (
    [Id]        INT           NOT NULL,
    [Login]     VARCHAR (255) NOT NULL,
    [FirstName] VARCHAR (30)  NOT NULL,
    [LastName]  VARCHAR (255) NOT NULL,
    [Mail]      VARCHAR (60)  NOT NULL,
    [Status]    INT           NOT NULL,
    [UpdatedOn] DATETIME      NOT NULL,
    [SynsDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

