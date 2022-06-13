CREATE TABLE [dbo].[NominationKPI] (
    [NominationId] INT            NOT NULL,
    [NominatorId]  INT            NOT NULL,
    [KPIGroupId]   INT            NOT NULL,
    [KPIId]        INT            NOT NULL,
    [Comment]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_NominationKPI] PRIMARY KEY CLUSTERED ([NominationId] ASC, [NominatorId] ASC, [KPIGroupId] ASC, [KPIId] ASC)
);

