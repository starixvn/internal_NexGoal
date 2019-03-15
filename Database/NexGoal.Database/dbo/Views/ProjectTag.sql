

CREATE VIEW [dbo].[ProjectTag]
AS
SELECT MIN(cv.CustomizedId) AS ProjectId
	,CAST(cv.Value AS NVARCHAR(MAX)) AS [Name]
FROM [$(Redmine)].dbo.CustomValues AS cv WITH (NOLOCK)
WHERE cv.CustomFieldId = 71
	AND cv.CustomizedType = 'Project'
	AND ISNULL(CAST(cv.Value AS NVARCHAR(MAX)), '') <> ''
	AND EXISTS (
		SELECT 1
		FROM [$(Redmine)].dbo.Projects AS p WITH (NOLOCK)
		WHERE p.Id = cv.CustomizedId
			AND p.[Status] = 1
		)
GROUP BY CAST(cv.Value AS NVARCHAR(MAX))

UNION ALL
SELECT - 1 AS ProjectId
	,'Probation' AS [Name]