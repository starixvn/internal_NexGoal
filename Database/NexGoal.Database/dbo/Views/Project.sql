
CREATE VIEW [dbo].[Project]
AS
SELECT p.Id AS ProjectId
	,CASE 
		WHEN cv.Value IS NOT NULL
			AND CAST(cv.Value AS NVARCHAR(MAX)) <> '' THEN CAST(cv.Value AS NVARCHAR(MAX))
		ELSE p.[Name]
		END AS [Name]
FROM [$(Redmine)].dbo.Projects AS p WITH (NOLOCK)
	LEFT JOIN [$(Redmine)].dbo.CustomValues AS cv WITH (NOLOCK) ON cv.CustomFieldId = 71
	AND cv.CustomizedType = 'Project'
	AND p.Id = cv.CustomizedId
