CREATE TABLE [dbo].[NominationKPIGroup] (
    [NominationId] INT           NOT NULL,
    [NominatorId]  INT           NOT NULL,
    [KPIGroupId]   INT           NOT NULL,
    [Rate]         VARCHAR (MAX) NULL,
    CONSTRAINT [PK_NominationKPIGroup] PRIMARY KEY CLUSTERED ([NominationId] ASC, [NominatorId] ASC, [KPIGroupId] ASC)
);

