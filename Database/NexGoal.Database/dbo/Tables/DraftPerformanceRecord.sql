CREATE TABLE [dbo].[DraftPerformanceRecord] (
    [DraftPerformanceRecordId] INT            IDENTITY (-1, -1) NOT NULL,
    [EmployeeIds]              VARCHAR (MAX)  NOT NULL,
    [GradeId]                  VARCHAR (MAX)  NULL,
    [Comment]                  NVARCHAR (MAX) NULL,
    [TypeId]                   TINYINT        NULL,
    [RoleId]                   TINYINT        NULL,
    [CreatorId]                INT            NULL,
    [ProjectIds]               VARCHAR (MAX)  NULL,
    [CreatedDate]              DATETIME       NULL,
    [ModifiedDate]             DATETIME       NULL,
    CONSTRAINT [DraftPerformanceRecordId] PRIMARY KEY CLUSTERED ([DraftPerformanceRecordId] ASC)
);

