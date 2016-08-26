

CREATE VIEW [dbo].[NexPM]
AS

	SELECT 
		tg.StartDate
		,tg.EndDate
		,STUFF((
				SELECT CAST(', ' AS VARCHAR(MAX)) + rl.NAME
				FROM NexEPA.dbo.ProjectMemberRole AS pmr WITH (NOLOCK)
					INNER JOIN NexEPA.dbo.ProjectRole AS rl WITH (NOLOCK) ON pmr.ProjectRoleId = rl.ProjectRoleId
				WHERE pmr.ProjectId = mb.ProjectId
					AND pmr.EmployeeId = mb.EmployeeId
				FOR XML PATH('')
				), 1, 1, '') AS Roles
		,epa.FinalScore AS Score
		,epa.FinalGrade As Grade
		,pc.Comment
		,pr.ProjectId
		,pr.NAME AS ProjectName
		,ee.EmployeeId
		,tp.ProjectManager
	FROM 
	NexEPA.dbo.[Target] AS tg WITH (NOLOCK)
		INNER JOIN NexEPA.dbo.TargetProject AS tp WITH (NOLOCK) ON tg.TargetId = tp.TargetId
		INNER JOIN NexEPA.dbo.EPA AS epa WITH (NOLOCK) ON tg.TargetId = epa.TargetId
		INNER JOIN NexEPA.dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		INNER JOIN NexEPA.dbo.Project AS pr WITH (NOLOCK) ON tp.ProjectId = pr.ProjectId
		INNER JOIN NexEPA.dbo.Employee AS ee WITH (NOLOCK) ON epa.EmployeeId = ee.EmployeeId
		LEFT JOIN NexEPA.dbo.ProjectMember AS mb WITH (NOLOCK) ON mb.EmployeeId = ee.EmployeeId AND pr.ProjectId = mb.ProjectId		

	WHERE  ee.IsDeleted = 0 AND ee.Enabled = 1