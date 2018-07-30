CREATE VIEW [dbo].[JobTitle]
AS
SELECT Id AS JobTitleId
	,[Name]
FROM NexHRM.dbo.LookupData WITH (NOLOCK)
WHERE LookupId = 1