
CREATE VIEW [dbo].[Project]
AS
SELECT Id AS ProjectId
	,[Name]
	,[Description]
	,ParentId
FROM [$(Redmine)].dbo.Projects WITH (NOLOCK)
