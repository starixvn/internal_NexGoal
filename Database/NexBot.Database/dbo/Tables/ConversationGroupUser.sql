CREATE TABLE [dbo].[ConversationGroupUser] (
    [GroupConversationId] VARCHAR (255) NOT NULL,
    [UserConversationId]  VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_ConversationGroupUser] PRIMARY KEY CLUSTERED ([GroupConversationId] ASC, [UserConversationId] ASC)
);

