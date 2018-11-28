CREATE PROCEDURE [dbo].[PARecord_GetByActor] @ActorId INT
	,@PeriodId INT
	,@GroupIds VARCHAR(MAX) = ''
AS
/*
Created: 20180920@Tim.Trinh
Task: Get PA by actor.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@HRProcessId TINYINT = 1
		,@PMProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 5
		-- PA status
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
	
	CREATE TABLE #tblGroupId(GroupId INT)

	CREATE TABLE #tblPARecordId (
		PARecordId INT
		,ProcessId TINYINT
		)
	
	INSERT INTO #tblGroupId(GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblPARecordId (
		PARecordId
		,ProcessId
		)
	SELECT pa.PARecordId
		,MAX(process.ProcessId)
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		AND period.PeriodId = @PeriodId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId > @HRProcessId
		AND process.EvaluatorId = @ActorId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
	GROUP BY pa.PARecordId

	SELECT pa.PARecordId
		,pa.PeriodId
		,pa.EmployeeId
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,(t.Listening + t.Reading) AS Toeic
		,(CASE 
			WHEN tbl.ProcessId <> @ReviewerProcessId THEN @PAClosed
			ELSE pa.StatusId END
			) AS StatusId
		,(CASE tbl.ProcessId
			WHEN @PMProcessId THEN NULL
			ELSE pa.GradeId END
			) AS GradeId
		,(CASE tbl.ProcessId
			WHEN @PMProcessId THEN NULL
			ELSE pa.Comment END
			) AS Comment
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		INNER JOIN dbo.TOEIC AS t WITH (NOLOCK) ON pa.EmployeeId = t.EmpId

	SELECT process.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,record.PerformanceRecordId
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
	WHERE process.ProcessId > @HRProcessId
		AND (
			tbl.ProcessId <> @PMProcessId
			OR (
				tbl.ProcessId = @PMProcessId
				AND process.EvaluatorId = @ActorId
				)
			)

	SELECT record.PerformanceRecordId
		,record.CreatorId
		,record.GradeId
		,record.Comment
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON record.PerformanceRecordId = process.PerformanceRecordId
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
	WHERE process.ProcessId > @HRProcessId
		AND (
			tbl.ProcessId <> @PMProcessId
			OR (
				tbl.ProcessId = @PMProcessId
				AND process.EvaluatorId = @ActorId
				)
			)
	
	DROP TABLE #tblGroupId
	DROP TABLE #tblPARecordId
END
