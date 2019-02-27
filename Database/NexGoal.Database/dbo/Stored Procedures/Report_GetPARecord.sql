CREATE PROCEDURE [dbo].[Report_GetPARecord] @ActorId INT
	,@PeriodIds VARCHAR(MAX) = ''
	,@GroupIds VARCHAR(MAX) = ''
	,@EmployeeIds VARCHAR(MAX) = ''
AS
/*
Created: 20181220@Oliver.Dang
Task: Get PA Report.
DB: bodb_NexGoal
Original:

Revisions:
    20190117@Tim.Trinh: refactor
Param's Explanation:

*/
BEGIN
	DECLARE @SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		,@EvaluatorProcessId TINYINT = 4
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@Today DATE = GETDATE()

	CREATE TABLE #tblGroupId (GroupId INT)

	CREATE TABLE #tblPARecordId (PARecordId INT)

	CREATE TABLE #tblPeriodId (PeriodId INT)

	CREATE TABLE #tblEmployee (EmpId INT)

	INSERT INTO #tblPeriodId (PeriodId)
	SELECT val
	FROM dbo.func_SplitString(@PeriodIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblGroupId (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblEmployee (EmpId)
	SELECT val
	FROM dbo.func_SplitString(@EmployeeIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblPARecordId (PARecordId)
	SELECT pa.PARecordId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
	INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
	INNER JOIN #tblPeriodId AS p WITH (NOLOCK) ON period.PeriodId = p.PeriodId
	INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.EvaluatorId = @ActorId
	INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
	INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	INNER JOIN #tblEmployee AS e WITH (NOLOCK) ON e.EmpId = emp.EmployeeId
	WHERE pa.StatusId IN (
			@PAInProgress
			,@PAClosed
			)
		AND process.ProcessId IN (
			@ReviewerProcessId
			,@EvaluatorProcessId
			,@SelfProcessId
			)
	GROUP BY pa.PARecordId

	SELECT pa.PARecordId
		,pa.Toeic
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,pa.IsRevised
		,pa.StatusId
		,pa.GradeId
		,pa.Comment
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,jobtitle.[Name] AS JobTitle
		,period.PeriodId
		,period.[Name]
	FROM dbo.PARecord AS pa WITH (NOLOCK)
	INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
	INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
	INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	INNER JOIN dbo.JobTitle AS jobtitle WITH (NOLOCK) ON emp.JobTitleId = jobtitle.JobTitleId
	INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
	ORDER BY (
			SELECT ISNULL(MIN(process.ProcessId), 0)
			FROM dbo.PAProcess AS process WITH (NOLOCK)
			WHERE process.PARecordId = pa.PARecordId
				AND process.ProcessId IN (
					@EvaluatorProcessId
					,@ReviewerProcessId
					,@SelfProcessId
					)
				AND process.EvaluatorId = @ActorId
			),
			emp.NickName

	SELECT process.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,record.PerformanceRecordId
		,record.GradeId
		,record.CreatedDate
		,record.ModifiedDate
		,record.Comment
		,process.ProcessId AS RoleId
		,record.TypeId
		,record.CreatorId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
	INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
	INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
	LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
	LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId

	SELECT p.PeriodId
		,p.Name
		,(
			CASE 
				WHEN p.EndDate >= @ToDay
					THEN 1
				ELSE 0
				END
			) AS IsInProgress
	FROM dbo.Period AS p WITH (NOLOCK)
	INNER JOIN #tblPeriodId AS tpl WITH (NOLOCK) ON p.PeriodId = tpl.PeriodId

	DROP TABLE #tblGroupId

	DROP TABLE #tblPARecordId

	DROP TABLE #tblPeriodId
END