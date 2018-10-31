
CREATE PROCEDURE [dbo].[PARecord_GetById] @ActorId INT
	,@PARecordId INT
AS
/*
Created: 20180920@Tim.Trinh
Task: Get PA by id.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@ProcessId TINYINT
		,@HRProcessId TINYINT = 1
		,@PMProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 5
		-- PA status
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	SELECT @ProcessId = MAX(process.ProcessId)
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId > @HRProcessId
		AND process.EvaluatorId = @ActorId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	WHERE pa.PARecordId = @PARecordId

	SELECT pa.PARecordId
		,pa.PeriodId
		,pa.EmployeeId
		,(CASE 
			WHEN @ProcessId <> @ReviewerProcessId THEN @PAClosed
			ELSE pa.StatusId END
			) AS StatusId
		,(CASE @ProcessId
			WHEN @PMProcessId THEN NULL
			ELSE pa.GradeId END
			) AS GradeId
		,(CASE @ProcessId
			WHEN @PMProcessId THEN NULL
			ELSE pa.Comment END
			) AS Comment
	FROM dbo.PARecord AS pa WITH (NOLOCK)
	WHERE pa.PARecordId = @PARecordId

	SELECT process.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,record.PerformanceRecordId
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
	WHERE process.PARecordId = @PARecordId
		AND process.ProcessId > @HRProcessId
		AND (
			@ProcessId <> @PMProcessId
			OR (
				@ProcessId = @PMProcessId
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
	WHERE process.PARecordId = @PARecordId
		AND process.ProcessId > @HRProcessId
		AND (
			@ProcessId <> @PMProcessId
			OR (
				@ProcessId = @PMProcessId
				AND process.EvaluatorId = @ActorId
				)
			)
END