CREATE TABLE [dbo].[PAFocus] (
    [PAFocusId]         INT            IDENTITY (1, 1) NOT NULL,
    [PeriodId]          INT            NOT NULL,
    [EmployeeId]        INT            NOT NULL,
    [LastObjectiveId]   INT            NULL,
    [NextObjectiveId]   INT            NULL,
    [IsReadyPromotion]  NVARCHAR (MAX) NULL,
    [IsPoorPerformance] NVARCHAR (MAX) NULL,
    [Contribution]      NVARCHAR (MAX) NULL,
    [BonusGiven]        NVARCHAR (MAX) NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([PAFocusId] ASC)
);

