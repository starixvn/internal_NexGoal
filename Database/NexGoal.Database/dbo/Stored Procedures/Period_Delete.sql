
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

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	IF EXISTS (SELECT 1
			   FROM dbo.PAProcess pap WITH (NOLOCK)
			   WHERE pap.PerformanceRecordId IS NOT NULL
				AND EXISTS (SELECT 1 FROM dbo.PARecord par WITH (NOLOCK) WHERE par.PeriodId = @PeriodId)
				)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The period has already generated PA.'
		RETURN
	END
	
	
	SET @xml = (
			SELECT p.PeriodId AS '@pi'
				,p.[Name] AS '@n'
				,p.TargetTypeId AS '@tti'
				,p.[Description] AS '@d'
				,p.StartDate AS '@sd'
				,p.EndDate AS '@ed'
				,p.DisciplineStartDate AS '@dsd'
				,p.DisciplineEndDate AS '@ded'
				,p.FocusStartDate AS '@fsd'
				,p.IsActive AS '@ia'
				,p.CreatedBy AS '@cb'
				,p.CreatedDate AS '@cd'
				,p.ModifiedBy AS '@mb'
				,p.ModifiedDate AS '@md'
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE pap
	FROM dbo.PAProcess pap
	WHERE EXISTS (SELECT 1
				  FROM dbo.PARecord par WITH (NOLOCK)
				  WHERE par.PARecordId = pap.PARecordId 
					AND par.PeriodId = @PeriodId)
	
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
