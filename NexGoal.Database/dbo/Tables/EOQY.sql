CREATE TABLE [dbo].[EOQY] (
    [EOQYId]              INT            IDENTITY (1, 1) NOT NULL,
    [Quarter]             TINYINT        NULL,
    [Year]                INT            NOT NULL,
    [IsQuarter]           BIT            NOT NULL,
    [StartDate]           DATE           NOT NULL,
    [EndDate]             DATE           NOT NULL,
    [Description]         NVARCHAR (200) NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    [RemindDate]          DATE           NULL,
    [EndDateForCommittee] DATE           NULL,
    CONSTRAINT [PK_EOQY] PRIMARY KEY CLUSTERED ([EOQYId] ASC)
);



