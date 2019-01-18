
CREATE PROCEDURE [dbo].[PARecord_GetByReviewer] @ActorId INT
	,@PeriodId INT
	,@GroupIds VARCHAR(MAX) = ''
AS
/*
Created: 20181127@Oliver.Dang
Task: Get PA by Reviewer.
DB: bodb_NexGoal
Original:

Revisions:
    20190117@Tim.Trinh: refactor
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATETIME = GETDATE()
		,@SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		-- PA status
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	CREATE TABLE #tblGroupId (GroupId INT)

	CREATE TABLE #tblPARecordId (PARecordId INT)

	INSERT INTO #tblGroupId (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblPARecordId (PARecordId)
	SELECT pa.PARecordId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.EvaluatorId = @ActorId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
		AND process.ProcessId = @ReviewerProcessId
	GROUP BY pa.PARecordId

	SELECT pa.PARecordId
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,pa.Toeic
		,(CASE 
			WHEN pa.StatusId = @PAInProgress
				AND (
					@Today BETWEEN p.StartDate
						AND p.EndDate
					) THEN @PAInProgress
			ELSE @PAClosed END
			) AS StatusId
		,pa.GradeId
		,pa.Comment
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.TOEIC AS t WITH (NOLOCK) ON pa.EmployeeId = t.EmpId
		INNER JOIN dbo.PAProcess AS p WITH (NOLOCK) ON pa.PARecordId = p.PARecordId
		AND p.ProcessId = @ReviewerProcessId

	SELECT process.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,record.PerformanceRecordId
		,record.GradeId
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
	WHERE process.ProcessId >= @SelfProcessId

	DROP TABLE #tblGroupId

	DROP TABLE #tblPARecordId
END
