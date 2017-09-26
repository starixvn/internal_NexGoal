CREATE TABLE [dbo].[EOYQuestion] (
    [QuestionId]   INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (500) NOT NULL,
    [IsActive]     BIT            NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_EOYQuestion] PRIMARY KEY CLUSTERED ([QuestionId] ASC)
);

