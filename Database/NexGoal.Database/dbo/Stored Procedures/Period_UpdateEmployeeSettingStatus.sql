
CREATE PROCEDURE [dbo].[Period_UpdateEmployeeSettingStatus] @PARecordId INT
	,@IsExcluded BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(2000) = '' OUTPUT
AS
/*
Created: 20180910@Elton.Ton
Task: update employee setting status.
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
	DECLARE @today DATE = GETDATE()
		,@PANotGenerated TINYINT = 1
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@PARejected TINYINT = 5
		,@periodEndDate DATE
	DECLARE @ReviewerProcessId TINYINT = 5

	SET @xml = (
			SELECT par.PARecordId AS '@pari'
				,par.StatusId AS '@si'
			FROM dbo.PARecord par WITH (NOLOCK)
			WHERE par.PARecordId = @PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	SELECT @periodEndDate = p.EndDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PARecord AS par WITH (NOLOCK)
			WHERE par.PeriodId = p.PeriodId
				AND par.PARecordId = @PARecordId
			)

	UPDATE par
	SET StatusId = (CASE 
			WHEN @IsExcluded = 1 THEN @PARejected
			WHEN @today >= reviewer.StartDate
				AND (
					@today <= reviewer.EndDate
					OR @today <= @periodEndDate
					) THEN @PAInProgress
			WHEN @periodEndDate < @today
				AND reviewer.EndDate < @today THEN @PAClosed
			WHEN @today < reviewer.StartDate THEN @PAOpened
			ELSE @PANotGenerated END
			)
	FROM dbo.PARecord AS par WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON par.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE par.PARecordId = @PARecordId

	SET @xml = (
			SELECT par.PARecordId AS '@pari'
				,par.StatusId AS '@si'
			FROM dbo.PARecord par WITH (NOLOCK)
			WHERE par.PARecordId = @PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_UpdateEmployeeSettingStatus'
		,@ObjectId = @PARecordId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END
