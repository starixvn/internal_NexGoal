CREATE TABLE [dbo].[Trackers] (
    [Id]       INT          NOT NULL,
    [Name]     VARCHAR (30) NOT NULL,
    [SynsDate] DATETIME     NOT NULL,
    CONSTRAINT [PK_Trackers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

