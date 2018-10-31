
CREATE PROCEDURE [dbo].[Period_UpdateEmployeeSettingStatus] @PARecordId INT
	,@StatusId TINYINT
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

	SET @xml = (
			SELECT par.PARecordId AS '@pari'
				,par.StatusId AS '@si'
			FROM dbo.PARecord par WITH (NOLOCK)
			WHERE par.PARecordId = @PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.PARecord WITH (ROWLOCK, UPDLOCK)
	SET StatusId = @StatusId
	WHERE PARecordId = @PARecordId

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