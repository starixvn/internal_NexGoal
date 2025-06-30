CREATE OR ALTER PROCEDURE [dbo].[SyncTOEIC_GetLastModifiedDate]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MAX(ModifiedDate) AS LastModifiedDate
    FROM [dbo].[TOEIC]
    WHERE ModifiedDate IS NOT NULL;
END 