CREATE TABLE [dbo].[ObjectiveEmployee] (
    [ObjectiveId]      INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]       INT            NULL,
    [PercentCompleted] NVARCHAR (MAX) NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [CreatedDate]      DATETIME       NULL,
    CONSTRAINT [PK_ObjectiveEmployee] PRIMARY KEY CLUSTERED ([ObjectiveId] ASC)
);

