
CREATE PROCEDURE [dbo].[Period_UpdateStatus] @PeriodId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(2000) = '' OUTPUT
AS
/*
Created: 20180731@Elton.Ton
Task: update Period status.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		
	SET @xml = (
			SELECT p.PeriodId AS '@pi'
				,p.IsActive AS '@ia'
				,p.ModifiedBy AS '@mb'
				,p.ModifiedDate AS '@md'
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.Period WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE PeriodId = @PeriodId

	SET @xml = (
			SELECT p.PeriodId AS '@pi'
				,p.IsActive AS '@ia'
				,p.ModifiedBy AS '@mb'
				,p.ModifiedDate AS '@md'
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_UpdateStatus'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END
