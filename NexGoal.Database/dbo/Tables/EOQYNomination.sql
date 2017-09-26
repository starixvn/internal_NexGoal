CREATE TABLE [dbo].[EOQYNomination] (
    [NominationId] INT      IDENTITY (1, 1) NOT NULL,
    [EOQYId]       INT      NOT NULL,
    [NomineeId]    INT      NOT NULL,
    [NominatorId]  INT      NOT NULL,
    [ApproverId]   INT      NOT NULL,
    [StatusId]     TINYINT  NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_EOQYNomination] PRIMARY KEY CLUSTERED ([NominationId] ASC)
);

