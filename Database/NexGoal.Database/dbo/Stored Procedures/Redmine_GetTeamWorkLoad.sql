CREATE PROCEDURE [dbo].[Redmine_GetTeamWorkLoad] @EmployeeId INT
	,@PeriodId INT
AS
/*
Created: 20181001@Oliver.Dang
Task: Get member workload under leader
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @toDate DATE
		,@fromDate DATE
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	BEGIN
		SELECT @fromDate = p.DisciplineStartDate
			,@toDate = p.DisciplineEndDate
		FROM dbo.Period p WITH (NOLOCK)
		WHERE p.PeriodId = @PeriodId
	END

	SELECT p.EmployeeId
		,MIN(p.[Priority]) AS [Priority]
	INTO #member
	FROM (
		SELECT m.EmployeeId
			,(CASE 
				WHEN ProcessId = @EvaluatorProcessId THEN 1
				WHEN ProcessId = @ReviewerProcessId THEN 2
				ELSE 3 END
				) AS [Priority]
		FROM dbo.func_GetMember(@EmployeeId) m
		WHERE m.ProcessId IS NOT NULL
		) p
	GROUP BY p.EmployeeId

	SELECT g.GroupId
		,g.[Name] AS GroupName
		,e.EmployeeId
		,e.NickName
		,p.[Name] AS ProjectName
		,p.ProjectId
		,SUM(pst.[Hours]) AS TotalHour
	FROM [dbo].Employee AS e WITH (NOLOCK)
		INNER JOIN [dbo].[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN #member AS m ON m.EmployeeId = e.EmployeeId
		LEFT JOIN dbo.ProjectSpentTime AS pst WITH (NOLOCK) ON e.EmployeeId = pst.EmployeeId
		LEFT JOIN dbo.Project AS p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
		AND (pst.SpentOn IS NULL OR pst.SpentOn BETWEEN @fromDate AND @toDate)
	GROUP BY g.GroupId
		,g.[Name]
		,e.EmployeeId
		,e.NickName
		,p.ProjectId
		,p.[Name]
		,m.[Priority]
	ORDER BY m.[Priority]
		,g.[Name]
		,e.NickName

	DROP TABLE #member
END
