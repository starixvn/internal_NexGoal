
CREATE VIEW [dbo].[ProjectMember]
AS
SELECT DISTINCT e.EmployeeId
	,m.ProjectId
	,mr.RoleId
FROM [$(Redmine)].dbo.Members AS m WITH (NOLOCK)
	INNER JOIN [$(Redmine)].dbo.MemberRoles AS mr WITH (NOLOCK) ON m.Id = mr.MemberId
	INNER JOIN [$(Redmine)].dbo.Users AS u WITH (NOLOCK) ON m.UserId = u.Id
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.DomainUsername LIKE '%' + u.[Login] + '%'
	AND u.[Login] <> ''
