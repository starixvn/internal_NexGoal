

CREATE VIEW [dbo].[Group]
AS
SELECT Id AS GroupId
	,[Name]
	,Code
	,ParentId
	,(CASE 
		WHEN DepartmentId IS NULL
			OR DepartmentId = 0 THEN 999
		WHEN DepartmentId IS NOT NULL THEN DepartmentId END
		) AS DepartmentId
	,LeaderId
	,AssistantLeaderId
	,ISNULL(Deleted, 0) AS IsDeleted
	,Rank
FROM [$(NexHRM)].dbo.EmployeeGroup WITH (NOLOCK)

