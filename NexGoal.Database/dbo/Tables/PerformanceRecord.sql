CREATE TABLE [dbo].[PerformanceRecord] (
    [PerformanceRecordId] INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeIds]         VARCHAR (MAX)  NOT NULL,
    [CreatorId]           INT            NULL,
    [GradeId]             VARCHAR (MAX)  NULL,
    [Comment]             NVARCHAR (MAX) NULL,
    [StatusId]            TINYINT        NULL,
    [ProjectIds]          VARCHAR (MAX)  NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedDate]        DATETIME       NULL,
    [TypeId]              TINYINT        NULL,
    [CopyFromId]          INT            NULL,
    [RoleId]              TINYINT        NULL,
    CONSTRAINT [PK_PerformanceRecord] PRIMARY KEY CLUSTERED ([PerformanceRecordId] ASC)
);

