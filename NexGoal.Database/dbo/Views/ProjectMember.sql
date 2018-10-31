
CREATE VIEW [dbo].[ProjectMember]
AS
SELECT DISTINCT e.EmployeeId
	,m.ProjectId
	,mr.RoleId
FROM [$(RedmineNexcel)].dbo.Members AS m WITH (NOLOCK)
	INNER JOIN [$(RedmineNexcel)].dbo.MemberRoles AS mr WITH (NOLOCK) ON m.Id = mr.MemberId
	INNER JOIN [$(RedmineNexcel)].dbo.Users AS u WITH (NOLOCK) ON m.UserId = u.Id
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.DomainUsername LIKE '%' + u.[Login] + '%'
	AND u.[Login] <> ''
