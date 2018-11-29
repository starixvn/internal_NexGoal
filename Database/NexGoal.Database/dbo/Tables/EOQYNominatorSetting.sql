CREATE TABLE [dbo].[EOQYNominatorSetting] (
    [EOQYId]      INT          NOT NULL,
    [NominatorId] INT          NOT NULL,
    [RoleIds]     VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_EOQYNominatorSetting] PRIMARY KEY CLUSTERED ([EOQYId] ASC, [NominatorId] ASC)
);

