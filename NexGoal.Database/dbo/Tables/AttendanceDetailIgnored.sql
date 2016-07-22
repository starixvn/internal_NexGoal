CREATE TABLE [dbo].[AttendanceDetailIgnored] (
    [WorkDate]     DATETIME NOT NULL,
    [EmployeeId]   INT      NOT NULL,
    [ViolationId]  INT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL
);

