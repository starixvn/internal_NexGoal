CREATE TABLE [dbo].[ConversationRegister] (
    [CategoryId]     INT           NOT NULL,
    [ConversationId] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_ConversationRegister] PRIMARY KEY CLUSTERED ([CategoryId] ASC, [ConversationId] ASC)
);

