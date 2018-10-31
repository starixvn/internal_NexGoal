
CREATE PROCEDURE [dbo].[EOQ_Delete] @EOQId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180711@Tim.Trinh
Task: Delete EOQ.
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
			WHERE EOQYId = @EOQId
				AND IsQuarter = 1
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
			SELECT @EOQId AS '@id'
				,[Quarter] AS '@q'
				,[Year] AS '@y'
				,[Description] AS '@d'
				,StartDateForNominator AS '@sdn'
				,EndDateForNominator AS '@edn'
				,RemindDateForNominator AS '@rdn'
				,EndDateForCommittee AS '@edc'
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId = @EOQId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE setting
	FROM dbo.EOQYNominatorSetting AS setting
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON setting.EOQYId = eoq.EOQYId
	WHERE eoq.EOQYId = @EOQId
		AND eoq.IsQuarter = 1

	DELETE
	FROM dbo.EOQY
	WHERE EOQYId = @EOQId
		AND IsQuarter = 1

	-- write log after delete
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'EOQ_Delete'
		,@ObjectId = @EOQId
		,@OldData = @logDataOld
		,@Duration = @Duration
END