CREATE OR ALTER PROCEDURE [dbo].[SyncGroup_GetLastModifiedDate]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MAX(ModifiedDate) AS LastModifiedDate
	FROM [dbo].[Group]
	WHERE ModifiedDate IS NOT NULL;
END