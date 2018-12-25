
CREATE PROCEDURE [dbo].[EOY_Update] @EOYId INT
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
Created: 20180718@Tim.Trinh
Task: update EOY.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 0
				AND EOQYId != @EOYId
				AND (
					@StartDateForNominator BETWEEN StartDateForNominator
						AND EndDateForNominator
					OR @EndDateForNominator BETWEEN StartDateForNominator
						AND EndDateForNominator
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Start Date/End Date overlaps on another EOY.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 0
				AND EOQYId != @EOYId
				AND [Year] = @Year
			)
	BEGIN
		SELECT @ErrCode = 2
			,@ErrMsg = N'Employee of year ' + CONVERT(NVARCHAR, @year) + ' already exists.'

		RETURN
	END

	DECLARE @Today DATE = GETDATE()
		-- For write log
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- Write log before update	
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

	UPDATE dbo.EOQY WITH (ROWLOCK, UPDLOCK)
	SET StartDateForNominator = @StartDateForNominator
		,EndDateForNominator = @EndDateForNominator
		,RemindDateForNominator = @RemindDateForNominator
		,EndDateForCommittee = @EndDateForCommittee
		,[Year] = @Year
		,[Description] = @Description
	WHERE EOQYId = @EOYId
		AND IsQuarter = 0

	IF @EndDateForNominator < @Today
		OR @EndDateForCommittee < @Today
	BEGIN
		EXEC dbo.Auto_Nomination_UpdateWhenFinished @EOYId
			,@ErrCode OUTPUT
			,@ErrMsg OUTPUT
	END

	-- write log after update
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
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'EOY_Update'
		,@ObjectId = @EOYId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END
