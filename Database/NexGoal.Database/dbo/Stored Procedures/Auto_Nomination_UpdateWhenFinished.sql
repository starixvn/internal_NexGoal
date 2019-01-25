
CREATE PROCEDURE [dbo].[Auto_Nomination_UpdateWhenFinished] @NominationPeriodId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180919@Tim.Trinh
Task: Update Nomination when finished.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATE = GETDATE()
		,@CreatedDate DATETIME = GETDATE()
		,@NominationId INT
		,@PerformanceRecordId INT

	CREATE TABLE #tblNomination (
		NominationId INT
		,EmployeeId INT
		,PerformanceRecordId INT
		,StatusId TINYINT
		)

	-- insert performance record
	INSERT INTO #tblNomination (
		NominationId
		,EmployeeId
		,PerformanceRecordId
		)
	SELECT nomination.NominationId
		,nomination.NomineeId
		,nomination.PerformanceRecordId
	FROM dbo.Nomination AS nomination WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EndDateForCommittee < @Today
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON nomination.NomineeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	WHERE ISNULL(@NominationPeriodId, period.EOQYId) = period.EOQYId

	UPDATE record
	SET CreatedDate = @CreatedDate
	FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblNomination AS nomination ON record.PerformanceRecordId = nomination.PerformanceRecordId

	DELETE
	FROM #tblNomination
	WHERE PerformanceRecordId IS NOT NULL

	WHILE EXISTS (
			SELECT 1
			FROM #tblNomination WITH (NOLOCK)
			)
	BEGIN
		SELECT TOP 1 @NominationId = NominationId
		FROM #tblNomination WITH (NOLOCK)

		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,CreatedDate
			,TypeId
			,RoleId
			)
		SELECT EmployeeId
			,- 1
			,@CreatedDate
			,5 -- system Nomination
			,255
		FROM #tblNomination WITH (NOLOCK)
		WHERE NominationId = @NominationId

		SET @PerformanceRecordId = SCOPE_IDENTITY()

		UPDATE nomination
		SET nomination.PerformanceRecordId = @PerformanceRecordId
		FROM dbo.Nomination AS nomination WITH (ROWLOCK, UPDLOCK)
		WHERE nomination.NominationId = @NominationId

		DELETE
		FROM #tblNomination
		WHERE NominationId = @NominationId
	END

	DROP TABLE #tblNomination
END