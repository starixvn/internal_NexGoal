
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

	DECLARE @HRProcessId TINYINT = 1
		,@ReviewerProcessId TINYINT = 5

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
	FROM dbo.Period p WITH (NOLOCK)
	WHERE EXISTS (
		SELECT 1
		FROM dbo.PARecord par WITH (NOLOCK)
		WHERE par.PeriodId = p.PeriodId AND par.PARecordId = @PARecordId
	)

	UPDATE par
	SET StatusId = (
		CASE 
			WHEN @IsExcluded = 1 THEN @PARejected
			WHEN NOT EXISTS (
				SELECT 1
				FROM dbo.PAProcess pap WITH (NOLOCK)
				WHERE pap.PARecordId = par.PARecordId AND pap.ProcessId <> @HRProcessId
					AND pap.PerformanceRecordId IS NOT NULL
			) THEN @PANotGenerated
			WHEN @today BETWEEN hr.StartDate
					AND reviewer.EndDate
				THEN @PAInProgress
			WHEN @periodEndDate < @today AND reviewer.EndDate < @today THEN @PAClosed
			WHEN @today < hr.StartDate THEN @PAOpened 
		END
	)
	FROM dbo.PARecord AS par WITH (ROWLOCK, UPDLOCK)
		CROSS APPLY (
			SELECT pap.StartDate
			FROM dbo.PAProcess pap WITH (NOLOCK)
			WHERE par.PARecordId = pap.PARecordId AND pap.ProcessId = @HRProcessId
		) hr
		CROSS APPLY (
			SELECT pap.EndDate
			FROM dbo.PAProcess pap WITH (NOLOCK)
			WHERE par.PARecordId = pap.PARecordId AND pap.ProcessId = @ReviewerProcessId
		) reviewer
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
