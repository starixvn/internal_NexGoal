CREATE TABLE [dbo].[EOQYNominatorSetting] (
    [EOQYId]       INT           NOT NULL,
    [NominatorId]  INT           NOT NULL,
    [RoleId]       NVARCHAR (10) NOT NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL,
    CONSTRAINT [PK_EOQYNominatorSetting] PRIMARY KEY CLUSTERED ([EOQYId] ASC, [NominatorId] ASC)
);



