CREATE TABLE [dbo].[EPA] (
    [EPAId]             INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]        INT            NOT NULL,
    [PeriodId]          INT            NOT NULL,
    [ScoreRatingTypeId] INT            NULL,
    [RatingTypeId]      INT            NULL,
    [Status]            TINYINT        NULL,
    [DisciplineScore]   FLOAT (53)     NULL,
    [FinalScore]        NVARCHAR (MAX) NULL,
    [Grade]             NVARCHAR (MAX) NULL,
    [HasMonthlyEPA]     BIT            NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    CONSTRAINT [PK_EPA_1] PRIMARY KEY CLUSTERED ([EPAId] ASC)
);



