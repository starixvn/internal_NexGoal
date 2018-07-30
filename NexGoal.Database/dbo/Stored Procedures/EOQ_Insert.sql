
CREATE PROCEDURE [dbo].[EOQ_Insert] @Quarter TINYINT
	,@Year INT
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
Created: 20180711@Tim.Trinh
Task: insert EOQ.
DB: bodb_NexGoal
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EOQYId INT

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 1
				AND [Quarter] = @Quarter
				AND [Year] = @Year
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'EOQ already exists.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 1
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

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	INSERT INTO dbo.EOQY (
		[Quarter]
		,[Year]
		,StartDateForNominator
		,EndDateForNominator
		,RemindDateForNominator
		,EndDateForCommittee
		,IsQuarter
		,[Description]
		)
	VALUES (
		@Quarter
		,@Year
		,@StartDateForNominator
		,@EndDateForNominator
		,@RemindDateForNominator
		,@EndDateForCommittee
		,1
		,@Description
		)

	SELECT @EOQYId = SCOPE_IDENTITY()

	-- copy latest nominator setting
	INSERT INTO dbo.EOQYNominatorSetting (
		EOQYId
		,RoleIds
		,NominatorId
		)
	SELECT DISTINCT @EOQYId
		,setting.RoleIds
		,setting.NominatorId
	FROM dbo.EOQYNominatorSetting AS setting WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON setting.EOQYId = eoq.EOQYId
	WHERE eoq.IsQuarter = 1
		AND eoq.EndDateForNominator IN (
			SELECT MAX(EndDateForNominator)
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId != @EOQYId
				AND IsQuarter = 1
			)

	-- write log after insert
	SET @xml = (
			SELECT @EOQYId AS '@id'
				,[Quarter] AS '@q'
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
		,@SPName = 'EOQ_Insert'
		,@ObjectId = @EOQYId
		,@Data = @logData
		,@Duration = @Duration
END