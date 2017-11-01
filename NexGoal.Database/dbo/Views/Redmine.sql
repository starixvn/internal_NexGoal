
CREATE VIEW [dbo].[Redmine]
AS
SELECT p.Id AS ProjectId
	,p.[Name] AS ProjectName
	,u.FirstName AS NickName
	,t.SpentOn
	,SUM([Hours]) AS [Hours]
FROM RedmineNexcel.dbo.TimeEntries AS t WITH (NOLOCK)
	INNER JOIN RedmineNexcel.dbo.Users AS u WITH (NOLOCK) ON u.Id = t.UserId
	INNER JOIN RedmineNexcel.dbo.[Projects] AS p WITH (NOLOCK) ON p.Id = t.ProjectId
GROUP BY p.Id
	,p.[Name]
	,u.FirstName
	,t.SpentOn

UNION
SELECT pr.Id AS ProjectId
	,pr.[Name] AS ProjectName
	,u.FirstName AS NickName
	,m.[Date] AS SpentOn
	,((DATEDIFF(SECOND, 0, m.EndHour) - DATEDIFF(SECOND, 0, m.StartHour)) / 3600.00) AS [Hours]
FROM [RedmineNexcel].[dbo].[Projects] AS pr WITH (NOLOCK)
	INNER JOIN [RedmineNexcel].[dbo].[Meetings] AS m WITH (NOLOCK) ON m.ProjectId = pr.Id
	INNER JOIN [RedmineNexcel].[dbo].[MeetingUsers] AS mu WITH (NOLOCK) ON mu.MeetingId = m.Id
	INNER JOIN [RedmineNexcel].[dbo].[Users] AS u WITH (NOLOCK) ON u.Id = mu.UserId;