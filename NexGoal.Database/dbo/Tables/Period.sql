CREATE TABLE [dbo].[Period] (
    [PeriodId]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (200) NOT NULL,
    [TargetTypeId]        TINYINT        NOT NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [StartDate]           DATE           NULL,
    [EndDate]             DATE           NULL,
    [DisciplineStartDate] DATE           NULL,
    [DisciplineEndDate]   DATE           NULL,
    [FocusStartDate]      DATE           NULL,
    [FocusEndDate]        DATE           NULL,
    [IsActive]            BIT            NOT NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    CONSTRAINT [PK_EvaluationPeriod] PRIMARY KEY CLUSTERED ([PeriodId] ASC) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];



