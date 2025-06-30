CREATE OR ALTER PROCEDURE [dbo].[SyncEmployeeOvertime_GetLastModifiedDate]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MAX(ModifiedDate) AS LastModifiedDate
    FROM [dbo].[EmployeeOvertime]
    WHERE ModifiedDate IS NOT NULL;
END 