
CREATE PROCEDURE [dbo].[Auto_PA_UpdateWhenEvaluationFinished] @ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180919@Tim.Trinh
Task: Update PA when evaluation finished.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@PARecordId INT
		,@PerformanceRecordId INT
	-- Performance Record Status
	DECLARE @Closed TINYINT = 4

	CREATE TABLE #tblPARecord (
		PARecordId INT
		,EmployeeId INT
		,GradeId VARCHAR(MAX)
		,PerformanceRecordId INT
		)

	INSERT INTO #tblPARecord (
		PARecordId
		,EmployeeId
		,GradeId
		,PerformanceRecordId
		)
	SELECT pa.PARecordId
		,pa.EmployeeId
		,pa.GradeId
		,pa.PerformanceRecordId
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE pa.StatusId IN (@PAOpened, @PAInProgress)
		AND (
			(
				period.EndDate > reviewer.EndDate
				AND period.EndDate < @Today
				)
			OR (
				period.EndDate < reviewer.EndDate
				AND reviewer.EndDate < @Today
				)
			)

	UPDATE record
	SET record.GradeId = pa.GradeId
		,record.StatusId = @PAClosed
		,record.CreatedDate = GETDATE()
	FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId

	DELETE
	FROM #tblPARecord
	WHERE PerformanceRecordId IS NOT NULL

	WHILE EXISTS (
			SELECT 1
			FROM #tblPARecord WITH (NOLOCK)
			)
	BEGIN
		SELECT TOP 1 @PARecordId = PARecordId
		FROM #tblPARecord WITH (NOLOCK)

		INSERT INTO dbo.PerformanceRecord (
			EmployeeIds
			,CreatorId
			,GradeId
			,StatusId
			,CreatedDate
			,TypeId
			,RoleId
			)
		SELECT EmployeeId
			,- 1
			,GradeId
			,@Closed
			,GETDATE()
			,4 -- system PA
			,255
		FROM #tblPARecord WITH (NOLOCK)
		WHERE PARecordId = @PARecordId

		SET @PerformanceRecordId = SCOPE_IDENTITY()

		UPDATE pa
		SET pa.StatusId = @PAClosed
			,pa.PerformanceRecordId = @PerformanceRecordId
		FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		WHERE pa.PARecordId = @PARecordId

		DELETE
		FROM #tblPARecord
		WHERE PARecordId = @PARecordId
	END

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to update PA when finished.'

		RETURN
	END

	DROP TABLE #tblPARecord
END