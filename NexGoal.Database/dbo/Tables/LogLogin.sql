CREATE TABLE [dbo].[LogLogin] (
    [IdentifyUser] VARCHAR (50)  NOT NULL,
    [LoginUser]    VARCHAR (50)  NOT NULL,
    [Roles]        VARCHAR (20)  NULL,
    [MachineName]  VARCHAR (100) NULL,
    [IPAddress]    VARCHAR (50)  NULL,
    [BrowserName]  VARCHAR (50)  NULL,
    [LoginDate]    DATETIME      NULL
);

