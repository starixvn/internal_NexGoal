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

	SELECT @fromDate = DisciplineStartDate
		,@toDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT ee.RankIndex
		,ee.EmployeeId
	INTO #sortedemployee
	FROM dbo.func_GetSortedEmployee(@EmployeeId) AS ee
		INNER JOIN dbo.func_GetMember(@EmployeeId) AS m ON ee.EmployeeId = m.EmployeeId

	SELECT g.GroupId
		,g.[Name] AS GroupName
		,e.EmployeeId
		,e.NickName
		,p.[Name] AS ProjectName
		,p.ProjectId
		,pst.[Hours] AS TotalHour
	FROM [dbo].Employee AS e WITH (NOLOCK)
		INNER JOIN [dbo].[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN #sortedemployee AS ee ON e.EmployeeId = ee.EmployeeId
		LEFT JOIN dbo.ProjectSpentTime AS pst WITH (NOLOCK) ON e.EmployeeId = pst.EmployeeId
		AND pst.SpentOn BETWEEN @fromDate
			AND @toDate
		LEFT JOIN dbo.Project AS p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
	ORDER BY ee.RankIndex

	DROP TABLE #sortedemployee
END

