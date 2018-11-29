
CREATE PROCEDURE [dbo].[NominationPeriod_Delete] @NominationPeriodId INT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181122@Tim.Trinh
Task: Delete nomination period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATE = GETDATE()

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId = @NominationPeriodId
				AND StartDateForNominator <= @Date
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Cannot delete an "In Progress" or "Closed" nomination period.'

		RETURN
	END

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- write log before delete
	SET @xml = (
			SELECT EOQYId AS '@id'
				,[Quarter] AS '@q'
				,[Year] AS '@y'
				,[Description] AS '@d'
				,StartDateForNominator AS '@sdn'
				,EndDateForNominator AS '@edn'
				,RemindDateForNominator AS '@rdn'
				,EndDateForCommittee AS '@edc'
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId = @NominationPeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE record
	FROM dbo.PerformanceRecord AS record
		INNER JOIN dbo.NominationDetail AS detail WITH (NOLOCK) ON record.PerformanceRecordId = detail.PerformanceRecordId
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON detail.NominationId = detail.NominationId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EOQYId = @NominationPeriodId

	DELETE detail
	FROM dbo.NominationDetail AS detail
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON detail.NominationId = detail.NominationId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EOQYId = @NominationPeriodId

	DELETE record
	FROM dbo.PerformanceRecord AS record
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EOQYId = @NominationPeriodId

	DELETE nomination
	FROM dbo.Nomination AS nomination
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EOQYId = @NominationPeriodId

	DELETE setting
	FROM dbo.EOQYNominatorSetting AS setting
		INNER JOIN dbo.EOQY AS eoq ON setting.EOQYId = eoq.EOQYId
	WHERE eoq.EOQYId = @NominationPeriodId

	DELETE
	FROM dbo.EOQY
	WHERE EOQYId = @NominationPeriodId

	-- write log after delete
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'NominationPeriod_Delete'
		,@ObjectId = @NominationPeriodId
		,@OldData = @logDataOld
		,@Duration = @Duration
END