CREATE TABLE [dbo].[PARecord] (
    [PARecordId]          INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]          INT            NULL,
    [PeriodId]            INT            NULL,
    [StatusId]            TINYINT        NULL,
    [GradeId]             VARCHAR (MAX)  NULL,
    [Comment]             NVARCHAR (MAX) NULL,
    [ToeicPolicy]         TINYINT        NULL,
    [PerformanceRecordId] INT            NULL,
    CONSTRAINT [PK_PARecord] PRIMARY KEY CLUSTERED ([PARecordId] ASC)
);



