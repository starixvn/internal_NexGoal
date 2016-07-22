

  CREATE VIEW [dbo].[Group]
  AS
  SELECT Id As GroupId,Name,Code,CASE WHEN DepartmentId IS NULL THEN 999 WHEN DepartmentId IS NOT NULL THEN DepartmentId END AS  DepartmentId
  ,LeaderId,AssistantLeaderId,Rank,ISNULL(Deleted,0) AS IsDeleted,1 AS lft,1 AS rgt
  From NexHRM.dbo.EmployeeGroup WITH(NOLOCK)