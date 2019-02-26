CREATE TABLE [dbo].[PARecord] (
    [PARecordId]          INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]          INT            NOT NULL,
    [PeriodId]            INT            NOT NULL,
    [StatusId]            TINYINT        NULL,
    [OriginalGradeId]     VARCHAR (MAX)  NULL,
    [GradeId]             VARCHAR (MAX)  NULL,
    [Comment]             NVARCHAR (MAX) NULL,
    [Toeic]               SMALLINT       NULL,
    [ToeicPolicy]         SMALLINT       NULL,
    [PerformanceRecordId] INT            NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    [IsRevised]           BIT            NULL,
    CONSTRAINT [PK_PARecord] PRIMARY KEY CLUSTERED ([PARecordId] ASC)
);









