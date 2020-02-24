CREATE PROCEDURE [dbo].[PRFeedbackReaction_Delete] @PRFeedbackId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: remove PR feedback reaction
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATETIME = GETDATE()

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PRFeedbackReaction WITH (NOLOCK)
		WHERE PRFeedbackId = @PRFeedbackId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The reaction is not existed.'

		RETURN
	END

	DELETE 
	FROM dbo.PRFeedbackReaction
	WHERE PRFeedbackId = @PRFeedbackId
	AND CreatorId = @CreatorId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PRFeedbackId AS '@id'
				,@CreatorId AS '@cid'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedbackReaction_Delete'
		,@Data = @logData
		,@Duration = @duration
END