CREATE VIEW [dbo].[Redmine]
AS
     SELECT p.Name AS ProjectName,
            u.FirstName AS NickName,
            t.SpentOn,
            SUM(Hours) AS Hours
     FROM RedmineNexcel.dbo.TimeEntries AS t WITH (NOLOCK)
          INNER JOIN RedmineNexcel.dbo.Users AS u WITH (Nolock) ON u.Id = t.UserId
          INNER JOIN RedmineNexcel.dbo.[Projects] AS p WITH (nolock) ON p.Id = t.ProjectId
     GROUP BY p.Name,
              u.FirstName,
              t.SpentOn
     UNION
     SELECT pr.Name AS ProjectName,
            u.FirstName AS NickName,
            m.Date AS SpentOn,
            ((DATEDIFF(SECOND, 0, m.EndHour) - DATEDIFF(SECOND, 0, m.StartHour)) / 3600.00) AS Hours
     FROM [RedmineNexcel].[dbo].[Projects] AS pr
          INNER JOIN [RedmineNexcel].[dbo].[Meetings] AS m ON m.ProjectId = pr.Id
          INNER JOIN [RedmineNexcel].[dbo].[MeetingUsers] AS mu ON mu.MeetingId = m.Id
          INNER JOIN [RedmineNexcel].[dbo].[Users] AS u ON u.Id = mu.UserId;