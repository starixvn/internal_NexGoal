CREATE TABLE [dbo].[Conversation] (
    [ConversationId]    VARCHAR (255)  NOT NULL,
    [ChannelId]         VARCHAR (50)   NULL,
    [ServiceUrl]        NVARCHAR (255) NULL,
    [IsGroup]           BIT            NULL,
    [IsActive]          BIT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [ModifiedDate]      DATETIME       NULL,
    [IsAdmin]           BIT            NULL,
    [ConversationBotId] VARCHAR (255)  NULL,
    CONSTRAINT [PK_Conversation] PRIMARY KEY CLUSTERED ([ConversationId] ASC)
);

