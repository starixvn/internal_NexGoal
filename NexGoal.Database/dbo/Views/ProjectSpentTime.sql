CREATE VIEW [dbo].[ProjectSpentTime]
AS
SELECT te.ProjectId
	,e.EmployeeId
	,te.[Hours]
	,te.SpentOn
FROM RedmineNexcel.dbo.TimeEntries AS te WITH (NOLOCK)
	INNER JOIN RedmineNexcel.dbo.Users AS u WITH (NOLOCK) ON te.UserId = u.Id
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.DomainUsername LIKE '%' + u.[Login] + '%'
	AND u.[Login] <> ''