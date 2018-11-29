
CREATE VIEW [dbo].[Project]
AS
SELECT Id AS ProjectId
	,[Name]
	,[Description]
	,ParentId
FROM [$(RedmineNexcel)].dbo.Projects WITH (NOLOCK)
