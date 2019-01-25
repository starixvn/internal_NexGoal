CREATE TABLE [dbo].[PerformanceRecord] (
    [PerformanceRecordId] INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]          INT            NULL,
    [CreatorId]           INT            NULL,
    [GradeId]             VARCHAR (MAX)  NULL,
    [Comment]             NVARCHAR (MAX) NULL,
    [ProjectIds]          VARCHAR (MAX)  NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedDate]        DATETIME       NULL,
    [TypeId]              TINYINT        NULL,
    [RoleId]              TINYINT        NULL,
    CONSTRAINT [PK_PerformanceRecord] PRIMARY KEY CLUSTERED ([PerformanceRecordId] ASC)
);

