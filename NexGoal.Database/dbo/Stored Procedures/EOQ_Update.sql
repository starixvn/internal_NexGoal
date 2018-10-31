
CREATE PROCEDURE [dbo].[EOQ_Update] @EOQId INT
	,@Quarter INT
	,@Year INT
	,@StartDateForNominator DATE
	,@EndDateForNominator DATE
	,@RemindDateForNominator DATE
	,@EndDateForCommittee DATE
	,@Description NVARCHAR(200) = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180711@Tim.Trinh
Task: Update EOQ.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 1
				AND EOQYId != @EOQId
				AND (
					@StartDateForNominator BETWEEN StartDateForNominator
						AND EndDateForNominator
					OR @EndDateForNominator BETWEEN StartDateForNominator
						AND EndDateForNominator
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Start Date/End Date overlaps on another EOQ.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 1
				AND EOQYId != @EOQId
				AND [Year] = @Year
				AND [Quarter] = @Quarter
			)
	BEGIN
		SELECT @ErrCode = 2
			,@ErrMsg = N'Employee of quater ' + CONVERT(NVARCHAR, @Quarter) + ' already exists.'

		RETURN
	END

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- Write log before update	
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

	UPDATE dbo.EOQY WITH (ROWLOCK, UPDLOCK)
	SET [Quarter] = @Quarter
		,[Year] = @Year
		,StartDateForNominator = @StartDateForNominator
		,EndDateForNominator = @EndDateForNominator
		,RemindDateForNominator = @RemindDateForNominator
		,EndDateForCommittee = @EndDateForCommittee
		,[Description] = @Description
	WHERE EOQYId = @EOQId
		AND IsQuarter = 1

	-- write log after update
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
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'EOQ_Update'
		,@ObjectId = @EOQId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END
