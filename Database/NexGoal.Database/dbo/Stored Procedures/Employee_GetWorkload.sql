CREATE PROCEDURE [dbo].[Employee_GetWorkload] @EmployeeId INT
	,@PeriodId INT = NULL
AS
/*
Created: 20181102@Oliver.Dang
Task: Get employee workload
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @toDate DATE
		,@fromDate DATE
		,@GroupId INT
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SET @GroupId = (
			SELECT GroupId
			FROM dbo.Employee
			WHERE EmployeeId = @EmployeeId
			)

	IF @PeriodId IS NULL
	BEGIN
		SET @toDate = GETDATE()
		SET @fromDate = DATEADD(MONTH, - 6, @toDate)
	END
	ELSE
	BEGIN
		SELECT @fromDate = DisciplineStartDate
			,@toDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
		FROM dbo.Period WITH (NOLOCK)
		WHERE PeriodId = @PeriodId
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@EmployeeId)
			WHERE ProcessId IS NOT NULL
				AND ProcessId = @EvaluatorProcessId
			) -- is Evaluator
	BEGIN
		SELECT s.ProjectId
			,p.[Name]
			,SUM(s.[Hours]) AS TotalHours
		FROM dbo.func_GetMember(@EmployeeId) AS e
			INNER JOIN dbo.ProjectSpentTime AS s WITH (NOLOCK) ON e.EmployeeId = s.EmployeeId
			INNER JOIN dbo.Project AS p WITH (NOLOCK) ON p.ProjectId = s.ProjectId
		WHERE s.SpentOn BETWEEN @fromDate
				AND @toDate
			AND e.ProcessId = @EvaluatorProcessId
		GROUP BY s.ProjectId
			,p.[Name]
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@EmployeeId)
			WHERE ProcessId IS NOT NULL
				AND ProcessId = @ReviewerProcessId
			) -- is Reviewer
	BEGIN
		SELECT s.ProjectId
			,p.[Name]
			,SUM(s.[Hours]) AS TotalHours
		FROM dbo.func_GetMember(@EmployeeId) AS e
			INNER JOIN dbo.ProjectSpentTime AS s WITH (NOLOCK) ON e.EmployeeId = s.EmployeeId
			INNER JOIN dbo.Project AS p WITH (NOLOCK) ON p.ProjectId = s.ProjectId
		WHERE s.SpentOn BETWEEN @fromDate
				AND @toDate
			AND e.ProcessId = @ReviewerProcessId
		GROUP BY s.ProjectId
			,p.[Name]
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@EmployeeId)
			WHERE ProcessId IS NOT NULL
				AND ProcessId = @PMProcessId
			) -- is PM
	BEGIN
		SELECT s.ProjectId
			,p.[Name]
			,SUM(s.[Hours]) AS TotalHours
		FROM dbo.func_GetMember(@EmployeeId) AS e
			INNER JOIN dbo.ProjectSpentTime AS s WITH (NOLOCK) ON e.EmployeeId = s.EmployeeId
			INNER JOIN dbo.Project AS p WITH (NOLOCK) ON p.ProjectId = s.ProjectId
		WHERE s.SpentOn BETWEEN @fromDate
				AND @toDate
			AND e.ProcessId = @PMProcessId
		GROUP BY s.ProjectId
			,p.[Name]
	END
	ELSE
	BEGIN
		SELECT pst.ProjectId
			,p.[Name]
			,SUM(pst.[Hours]) AS TotalHours
		FROM dbo.ProjectSpentTime AS pst WITH (NOLOCK)
			INNER JOIN dbo.Project AS p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
		WHERE pst.EmployeeId = @EmployeeId
			AND pst.SpentOn BETWEEN @fromDate
				AND @toDate
		GROUP BY pst.ProjectId
			,p.[Name]
	END
END
