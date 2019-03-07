CREATE TABLE [dbo].[ConversationBot] (
    [ConversationBotId] VARCHAR (255)  NOT NULL,
    [Name]              NVARCHAR (255) NULL,
    [ChannelId]         VARCHAR (50)   NULL,
    CONSTRAINT [PK_ConversationBot] PRIMARY KEY CLUSTERED ([ConversationBotId] ASC)
);

