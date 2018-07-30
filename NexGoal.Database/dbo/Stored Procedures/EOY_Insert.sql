
CREATE PROCEDURE [dbo].[EOY_Insert] @Year INT
	,@StartDateForNominator DATE
	,@EndDateForNominator DATE
	,@RemindDateForNominator DATE
	,@EndDateForCommittee DATE
	,@Description NVARCHAR(200) = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180718@Tim.Trinh
Task: insert EOY.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EOQYId INT
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 0
				AND [Year] = @Year
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'EOY already existes.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 0
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

	INSERT INTO dbo.EOQY (
		[Year]
		,StartDateForNominator
		,EndDateForNominator
		,RemindDateForNominator
		,EndDateForCommittee
		,IsQuarter
		,[Description]
		)
	VALUES (
		@Year
		,@StartDateForNominator
		,@EndDateForNominator
		,@RemindDateForNominator
		,@EndDateForCommittee
		,0
		,@Description
		)

	SELECT @EOQYId = SCOPE_IDENTITY()

	-- copy last nominator setting
	INSERT INTO dbo.EOQYNominatorSetting (
		EOQYId
		,RoleIds
		,NominatorId
		)
	SELECT DISTINCT @EOQYId
		,setting.RoleIds
		,setting.NominatorId
	FROM dbo.EOQYNominatorSetting AS setting WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS eoy WITH (NOLOCK) ON setting.EOQYId = eoy.EOQYId
	WHERE eoy.IsQuarter = 0
		AND eoy.EndDateForNominator IN (
			SELECT MAX(EndDateForNominator)
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId != @EOQYId
				AND IsQuarter = 0
			)
	
	-- write log after insert
	SET @xml = (
			SELECT @EOQYId AS '@id'
				,[Year] AS '@y'
				,[Description] AS '@d'
				,StartDateForNominator AS '@sdn'
				,EndDateForNominator AS '@edn'
				,RemindDateForNominator AS '@rdn'
				,EndDateForCommittee AS '@edc'
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId = @EOQYId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'EOY_Insert'
		,@ObjectId = @EOQYId
		,@Data = @logData
		,@Duration = @Duration
END