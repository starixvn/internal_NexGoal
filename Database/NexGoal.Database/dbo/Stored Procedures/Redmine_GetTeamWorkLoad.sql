
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
		,pst.[Hours] AS TotalHour
	FROM [dbo].Employee AS e WITH (NOLOCK)
		INNER JOIN [dbo].[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN #member AS m ON m.EmployeeId = e.EmployeeId
		LEFT JOIN dbo.ProjectSpentTime AS pst WITH (NOLOCK) ON e.EmployeeId = pst.EmployeeId
			AND pst.SpentOn BETWEEN @fromDate AND @toDate
			
		LEFT JOIN dbo.Project AS p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
		INNER JOIN dbo.func_GetSortedEmployee(@EmployeeId) AS ee ON e.EmployeeId = ee.EmployeeId
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
	ORDER BY ee.RankIndex

	DROP TABLE #member
END