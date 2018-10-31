
CREATE PROCEDURE [dbo].[EOY_Delete] @EOYId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180718@Tim.Trinh
Task: Delete EOY.
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
			WHERE EOQYId = @EOYId
				AND IsQuarter = 0
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
			SELECT @EOYId AS '@id'
				,[Year] AS '@y'
				,[Description] AS '@d'
				,StartDateForNominator AS '@sdn'
				,EndDateForNominator AS '@edn'
				,RemindDateForNominator AS '@rdn'
				,EndDateForCommittee AS '@edc'
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId = @EOYId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE setting
	FROM dbo.EOQYNominatorSetting AS setting
		INNER JOIN dbo.EOQY AS eoq ON setting.EOQYId = eoq.EOQYId
	WHERE eoq.EOQYId = @EOYId
		AND eoq.IsQuarter = 0

	DELETE
	FROM dbo.EOQY
	WHERE EOQYId = @EOYId
		AND IsQuarter = 0

	-- write log after delete
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'EOY_Delete'
		,@ObjectId = @EOYId
		,@OldData = @logDataOld
		,@Duration = @Duration
END
