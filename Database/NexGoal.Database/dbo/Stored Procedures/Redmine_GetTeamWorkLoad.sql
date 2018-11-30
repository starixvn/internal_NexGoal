CREATE PROCEDURE [dbo].[Redmine_GetTeamWorkLoad] 
	@EmployeeId INT
	,@PeriodId  INT
AS
/*
Created: 20181001@Oliver.Dang
Task: Get member workload under leader
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;


	DECLARE @toDate DATE, @fromDate DATE
	DECLARE @PMProcessId TINYINT = 3, @EvaluatorProcessId TINYINT = 4, @ReviewerProcessId TINYINT = 5

	BEGIN
		SELECT @fromDate = p.DisciplineStartDate, @toDate = p.DisciplineEndDate
		FROM dbo.Period p WITH (NOLOCK)
		WHERE p.PeriodId = @PeriodId
	END

	SELECT p.EmployeeId
		,MIN(p.Priority) AS Priority
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
		 ,g.Name as GroupName
		 ,e.EmployeeId
		 ,e.NickName
		 ,p.Name as ProjectName
		 ,p.ProjectId
		 ,SUM(pst.[Hours]) as TotalHour
	FROM [dbo].Employee as e WITH (NOLOCK)
	INNER JOIN [dbo].[Group] as g WITH (NOLOCK) ON e.GroupId = g.GroupId
	INNER JOIN dbo.ProjectSpentTime pst WITH (NOLOCK) ON e.EmployeeId = pst.EmployeeId
	INNER JOIN dbo.Project p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
	INNER JOIN #member m ON m.EmployeeId = e.EmployeeId
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
		AND pst.SpentOn BETWEEN @fromDate AND @toDate
	GROUP BY g.GroupId
		,g.Name
		,e.EmployeeId
		,e.NickName
		,p.ProjectId
		,p.Name
		,m.Priority
	ORDER BY m.Priority, g.Name, e.NickName
	
	DROP TABLE #member
END