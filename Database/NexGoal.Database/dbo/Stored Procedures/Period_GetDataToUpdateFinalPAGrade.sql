
CREATE PROCEDURE [dbo].[Period_GetDataToUpdateFinalPAGrade] @PeriodId INT
	,@PARecordIds VARCHAR(MAX) = NULL
AS
/*
Created: 201902193@Tim.Trinh
Task: get data to update final grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@EvaluatorProcessId TINYINT = 4

	CREATE TABLE #tblPARecordIds (PARecordId INT)

	INSERT INTO #tblPARecordIds (PARecordId)
	SELECT val
	FROM dbo.func_SplitString(@PARecordIds, ',')
	WHERE ISNUMERIC(val) = 1

	SELECT pa.PARecordId
		,pa.OriginalGradeId
		,pa.GradeId
		,pa.StatusId
		,ISNULL(toeic.Score, 0) AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND pa.PeriodId = @PeriodId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE pa.StatusId = @PAClosed
		AND (
			@PARecordIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tblPARecordIds AS tbl
				WHERE pa.PARecordId = tbl.PARecordId
				)
			)
	
	UNION
	-- reset grade if reviewer is not revised
	SELECT pa.PARecordId
		,NULL AS OriginalGradeId
		,record.GradeId
		,pa.StatusId
		,ISNULL(toeic.Score, 0) AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND pa.PeriodId = @PeriodId
		INNER JOIN dbo.PAProcess AS paprocess WITH (NOLOCK) ON pa.PARecordId = paprocess.PARecordId
		AND paprocess.ProcessId = @EvaluatorProcessId
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON paprocess.PerformanceRecordId = record.PerformanceRecordId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE pa.StatusId = @PAInProgress
		AND pa.ModifiedBy IS NULL
		AND ISNULL(pa.IsRevised, 0) = 0
		AND (
			@PARecordIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tblPARecordIds AS tbl
				WHERE pa.PARecordId = tbl.PARecordId
				)
			)

	DROP TABLE #tblPARecordIds
END