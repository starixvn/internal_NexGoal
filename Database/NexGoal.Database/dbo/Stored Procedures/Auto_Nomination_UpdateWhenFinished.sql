
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

	DECLARE @Today DATE = GETDATE()
		,@NominationId INT
		,@PerformanceRecordId INT
	-- Performance Record Status
	DECLARE @PR_Draft TINYINT = 2
		,@PR_Submitted TINYINT = 3
		,@PR_Closed TINYINT = 4

	CREATE TABLE #tblNomination (
		NominationId INT
		,EmployeeId INT
		,PerformanceRecordId INT
		,StatusId TINYINT
		)

	-- update performance record status
	UPDATE record
	SET record.StatusId = @PR_Closed
	FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.NominationDetail AS detail WITH (NOLOCK) ON record.PerformanceRecordId = detail.PerformanceRecordId
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON detail.NominationId = nomination.NominationId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EndDateForNominator < @Today
	WHERE record.StatusId = @PR_Submitted

	-- insert performance record
	INSERT INTO #tblNomination (
		NominationId
		,EmployeeId
		,PerformanceRecordId
		,StatusId
		)
	SELECT nomination.NominationId
		,nomination.NomineeId
		,nomination.PerformanceRecordId
		,nomination.StatusId
	FROM dbo.Nomination AS nomination WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EndDateForCommittee < @Today
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON nomination.NomineeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	WHERE ISNULL(@NominationPeriodId, period.EOQYId) = period.EOQYId
		AND nomination.PerformanceRecordId IS NULL

	WHILE EXISTS (
			SELECT 1
			FROM #tblNomination WITH (NOLOCK)
			)
	BEGIN
		SELECT TOP 1 @NominationId = NominationId
		FROM #tblNomination WITH (NOLOCK)

		INSERT INTO dbo.PerformanceRecord (
			EmployeeIds
			,CreatorId
			,StatusId
			,CreatedDate
			,TypeId
			,RoleId
			)
		SELECT EmployeeId
			,- 1
			,@PR_Closed
			,GETDATE()
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

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to update Nomination when finished.'

		RETURN
	END

	DROP TABLE #tblNomination
END