CREATE TABLE [dbo].[Period] (
    [PeriodId]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (200) NOT NULL,
    [TargetTypeId]        INT            NOT NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [StartDate]           DATETIME       NULL,
    [EndDate]             DATETIME       NULL,
    [DisciplineStartDate] DATETIME       NULL,
    [DisciplineEndDate]   DATETIME       NULL,
    [FocusStartDate]      DATETIME       NULL,
    [IsActive]            BIT            NOT NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    CONSTRAINT [PK_EvaluationPeriod] PRIMARY KEY CLUSTERED ([PeriodId] ASC) ON [PRIMARY]
) TEXTIMAGE_ON [PRIMARY];

