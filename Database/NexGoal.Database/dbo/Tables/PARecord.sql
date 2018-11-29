CREATE TABLE [dbo].[PARecord] (
    [PARecordId]          INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]          INT            NULL,
    [PeriodId]            INT            NULL,
    [StatusId]            TINYINT        NULL,
    [OriginalGradeId]     VARCHAR (MAX)  NULL,
    [GradeId]             VARCHAR (MAX)  NULL,
    [Comment]             NVARCHAR (MAX) NULL,
    [ToeicPolicy]         SMALLINT       NULL,
    [PerformanceRecordId] INT            NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    CONSTRAINT [PK_PARecord] PRIMARY KEY CLUSTERED ([PARecordId] ASC)
);





