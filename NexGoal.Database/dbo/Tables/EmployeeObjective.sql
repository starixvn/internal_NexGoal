CREATE TABLE [dbo].[EmployeeObjective] (
    [ObjectiveId]      INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]       INT            NOT NULL,
    [PercentCompleted] NVARCHAR (MAX) NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [CreatedDate]      DATE           NULL
);

