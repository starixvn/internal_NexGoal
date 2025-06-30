CREATE OR ALTER PROCEDURE [dbo].[SyncEmployeeCalendar_GetLastModifiedDate]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MAX(ModifiedDate) AS LastModifiedDate
    FROM [dbo].[EmployeeCalendar]
    WHERE ModifiedDate IS NOT NULL;
END 