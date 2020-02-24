CREATE PROCEDURE [dbo].[PRReaction_Delete] @PerformanceRecordId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: remove member's PR agreement
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PRReaction WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The reaction is not existed.'

		RETURN
	END

	DELETE 
	FROM dbo.PRReaction
	WHERE PerformanceRecordId = @PerformanceRecordId
		AND CreatorId = @CreatorId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordId AS '@id'
				,@CreatorId AS '@cid'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRReaction_Delete'
		,@Data = @logData
		,@Duration = @duration
END