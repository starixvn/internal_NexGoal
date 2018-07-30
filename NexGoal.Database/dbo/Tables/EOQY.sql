CREATE TABLE [dbo].[EOQY] (
    [EOQYId]                 INT            IDENTITY (1, 1) NOT NULL,
    [Quarter]                TINYINT        NULL,
    [Year]                   INT            NOT NULL,
    [IsQuarter]              BIT            NOT NULL,
    [Description]            NVARCHAR (200) NULL,
    [StartDateForNominator]  DATE           NOT NULL,
    [EndDateForNominator]    DATE           NOT NULL,
    [RemindDateForNominator] DATE           NULL,
    [EndDateForCommittee]    DATE           NULL,
    CONSTRAINT [PK_EOQY] PRIMARY KEY CLUSTERED ([EOQYId] ASC)
);

