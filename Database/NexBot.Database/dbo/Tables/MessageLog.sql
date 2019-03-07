CREATE TABLE [dbo].[MessageLog] (
    [ConversationId]     VARCHAR (255)   NULL,
    [FromConversationId] VARCHAR (255)   NULL,
    [FromName]           NVARCHAR (255)  NULL,
    [MessageContent]     NVARCHAR (2000) NULL,
    [InsertedDate]       DATETIME        NULL
);

