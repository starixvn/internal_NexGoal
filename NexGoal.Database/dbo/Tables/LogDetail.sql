CREATE TABLE [dbo].[LogDetail] (
    [LogId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [ActionId] SMALLINT       NULL,
    [ActorId]  INT            NULL,
    [ObjectId] INT            NULL,
    [LogDate]  DATETIME       NULL,
    [Duration] INT            NULL,
    [Data]     NVARCHAR (MAX) NULL,
    [OldData]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_LogDetail] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

