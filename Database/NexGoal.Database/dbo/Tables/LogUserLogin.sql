CREATE TABLE [dbo].[LogUserLogin] (
    [Username]    VARCHAR (50)  NOT NULL,
    [LogTime]     DATETIME      NOT NULL,
    [IP]          VARCHAR (30)  NULL,
    [MachineName] VARCHAR (100) NULL,
    [Platform]    VARCHAR (255) NULL
);

