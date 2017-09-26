CREATE TABLE [dbo].[EOQYNominationDetail] (
    [NominationId] INT            NOT NULL,
    [QuestionId]   INT            NOT NULL,
    [Comment]      NVARCHAR (MAX) NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_EOQYNominationDetail] PRIMARY KEY CLUSTERED ([NominationId] ASC, [QuestionId] ASC)
);

