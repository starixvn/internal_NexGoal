
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
		,@ReviewerProcessId TINYINT = 5

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
		AND @PeriodId = @PeriodId
		AND period.IsActive = 1
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
	SELECT pa.PARecordId
		,pa.OriginalGradeId
		,pa.OriginalGradeId AS GradeId
		,pa.StatusId
		,ISNULL(toeic.Score, 0) AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND @PeriodId = @PeriodId
		AND period.IsActive = 1
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE pa.StatusId = @PAInProgress
		AND pa.ModifiedBy IS NULL
		AND pa.IsRevised IS NULL
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