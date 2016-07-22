CREATE TABLE [dbo].[PeriodExcludedEmployee] (
    [PeriodId]     INT            NOT NULL,
    [EmployeeId]   INT            NOT NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL
);

