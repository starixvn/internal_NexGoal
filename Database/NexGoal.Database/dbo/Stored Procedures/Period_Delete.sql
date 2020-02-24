
CREATE PROCEDURE [dbo].[Period_Delete] @PeriodId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180731@Elton.Ton
Task: delete period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	DECLARE	@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	IF EXISTS (
			SELECT 1
			FROM dbo.PARecord WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND StatusId IN (@PAOpened, @PAInProgress, @PAClosed)
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The period has already generated PA.'

		RETURN
	END

	SET @xml = (
			SELECT PeriodId AS '@pi'
				,[Name] AS '@n'
				,TargetTypeId AS '@tti'
				,[Description] AS '@d'
				,StartDate AS '@sd'
				,EndDate AS '@ed'
				,DisciplineStartDate AS '@dsd'
				,DisciplineEndDate AS '@ded'
				,FocusStartDate AS '@fsd'
				,IsActive AS '@ia'
				,CreatedBy AS '@cb'
				,CreatedDate AS '@cd'
				,ModifiedBy AS '@mb'
				,ModifiedDate AS '@md'
			FROM dbo.Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE pap
	FROM dbo.PAProcess AS pap
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PARecord AS par WITH (NOLOCK)
			WHERE par.PARecordId = pap.PARecordId
				AND par.PeriodId = @PeriodId
			)

	DELETE dbo.PARecord
	WHERE PeriodId = @PeriodId

	DELETE dbo.Period
	WHERE PeriodId = @PeriodId

	DELETE dbo.PeriodProcessSetting
	WHERE PeriodId = @PeriodId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_Delete'
		,@ObjectId = @PeriodId
		,@OldData = @logDataOld
		,@Duration = @Duration
END
