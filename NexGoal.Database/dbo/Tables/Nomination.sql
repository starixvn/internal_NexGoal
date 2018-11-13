CREATE TABLE [dbo].[Nomination] (
    [NominationId]        INT     IDENTITY (1, 1) NOT NULL,
    [EOQYId]              INT     NOT NULL,
    [NomineeId]           INT     NOT NULL,
    [StatusId]            TINYINT NOT NULL,
    [PerformanceRecordId] INT     NULL,
    CONSTRAINT [PK_Nomination] PRIMARY KEY CLUSTERED ([NominationId] ASC)
);



