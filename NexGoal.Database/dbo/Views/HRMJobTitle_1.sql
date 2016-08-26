
  CREATE VIEW [HRMJobTitle]
  AS
SELECT Id, LookupId,Code,Name,Value,GetDate() AS SyncDate
FROM NexHRM.dbo.LookupData WITH(NOLOCK) WHERE LookupId=1