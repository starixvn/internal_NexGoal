
CREATE VIEW [dbo].[Project]
AS
SELECT Id AS ProjectId
	,[Name]
	,[Description]
	,ParentId
FROM [$(Redmine)].dbo.Projects WITH (NOLOCK)
WHERE [Status] = 1

UNION ALL
SELECT - 1  AS ProjectId
	,'Probation' AS [Name]
	,NULL AS [Description]
	,NULL AS ParentId
