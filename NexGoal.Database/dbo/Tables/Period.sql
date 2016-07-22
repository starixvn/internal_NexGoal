CREATE TABLE [dbo].[Period] (
    [PeriodId]            INT            NOT NULL,
    [Name]                NVARCHAR (200) NOT NULL,
    [TargetTypeId]        INT            NOT NULL,
    [StartDate]           DATETIME       NULL,
    [EndDate]             DATETIME       NULL,
    [AttendanceStartDate] DATETIME       NULL,
    [AttendanceEndDate]   DATETIME       NULL,
    [IsActive]            BIT            NOT NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL
);

