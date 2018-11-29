CREATE TABLE [dbo].[NominationDetail] (
    [NominationId]        INT NOT NULL,
    [NominatorId]         INT NOT NULL,
    [PerformanceRecordId] INT NOT NULL,
    CONSTRAINT [PK_NominationDetail] PRIMARY KEY CLUSTERED ([NominationId] ASC, [NominatorId] ASC)
);

