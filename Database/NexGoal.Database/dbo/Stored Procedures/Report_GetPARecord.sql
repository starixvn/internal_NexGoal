CREATE PROCEDURE [dbo].[Report_GetPARecord] @ActorId INT
	,@PeriodIds VARCHAR(MAX) = ''
	,@GroupIds VARCHAR(MAX) = ''
	,@EmployeeId INT = NULL
AS
/*
Created: 20181220@Oliver.Dang
Task: Get PA by Reviewer.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @ReviewerProcessId TINYINT = 5
		,@EvaluatorProcessId TINYINT = 4
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
	
	CREATE TABLE #tblGroupId(GroupId INT)

	CREATE TABLE #tblPARecordId (
		PARecordId INT
		)
	CREATE TABLE #tblPeriodId(PeriodId INT)

	INSERT INTO #tblPeriodId(PeriodId)
	SELECT val
	FROM dbo.func_SplitString(@PeriodIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblGroupId(GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblPARecordId (
		PARecordId
		)
	SELECT pa.PARecordId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN #tblPeriodId AS p WITH (NOLOCK) ON period.PeriodId = p.PeriodId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND (process.EvaluatorId = @ActorId)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
		AND process.ProcessId IN (@ReviewerProcessId, @EvaluatorProcessId) 
		AND (@EmployeeId IS NULL OR @EmployeeId = emp.EmployeeId)
	GROUP BY pa.PARecordId
	UNION
	SELECT pa.PARecordId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN #tblPeriodId AS p WITH (NOLOCK) ON period.PeriodId = p.PeriodId
	WHERE pa.EmployeeId = @ActorId AND @EmployeeId IS NULL
	GROUP BY pa.PARecordId

	SELECT pa.PARecordId
		,pa.PeriodId
		,pa.EmployeeId
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,pa.Toeic
		,pa.StatusId 
		,pa.GradeId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId 

	SELECT process.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,record.PerformanceRecordId
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId

	SELECT record.PerformanceRecordId
		,record.CreatorId
		,record.GradeId
		,record.CreatedDate
		,record.ModifiedDate
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON record.PerformanceRecordId = process.PerformanceRecordId
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
	
	DROP TABLE #tblGroupId
	DROP TABLE #tblPARecordId
	DROP TABLE #tblPeriodId

END
