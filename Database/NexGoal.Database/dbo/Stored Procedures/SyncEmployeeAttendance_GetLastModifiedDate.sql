CREATE OR ALTER PROCEDURE [dbo].[SyncEmployeeAttendance_GetLastModifiedDate]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MAX(InsertDate) AS LastModifiedDate
	FROM [dbo].[EmployeeAttendance]
	WHERE InsertDate IS NOT NULL;
END