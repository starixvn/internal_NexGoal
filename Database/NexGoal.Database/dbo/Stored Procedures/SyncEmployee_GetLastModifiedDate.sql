CREATE OR ALTER PROCEDURE [dbo].[SyncEmployee_GetLastModifiedDate]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MAX(ModifiedDate) AS LastModifiedDate
	FROM [dbo].[Employee]
	WHERE ModifiedDate IS NOT NULL;
END