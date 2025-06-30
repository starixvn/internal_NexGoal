CREATE OR ALTER PROCEDURE [dbo].[SyncWorkingTimeRequest_GetLastModifiedDate]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MAX(ModifiedDate) AS LastModifiedDate
    FROM [dbo].[WorkingTimeRequest]
    WHERE ModifiedDate IS NOT NULL;
END 