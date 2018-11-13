
CREATE PROCEDURE [dbo].[Auto_Nomination_UpdateWhenFinished] @ErrCode INT = 0 OUTPUT
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
	-- Nomination Status
	DECLARE @Draft TINYINT = 4
	-- Performance Record Status
	DECLARE @Closed TINYINT = 4

	CREATE TABLE #tblNomination (
		NominationId INT
		,EmployeeId INT
		,PerformanceRecordId INT
		,StatusId TINYINT
		)

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
	WHERE nomination.PerformanceRecordId IS NULL

	DELETE
	FROM #tblNomination
	WHERE StatusId = @Draft

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
			,@Closed
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