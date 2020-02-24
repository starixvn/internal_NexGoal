CREATE PROCEDURE [dbo].[PRFeedback_Update] @PRFeedbackId INT
	,@CreatorId INT
	,@Content NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Edit comment on member's PR
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATETIME = GETDATE()
	DECLARE @OldContent NVARCHAR(MAX)

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PRFeedback WITH (NOLOCK)
		WHERE PRFeedbackId = @PRFeedbackId
			AND CreatorId = @CreatorId
			AND DATEDIFF(DAY, CreatedDate, @Today) <= 7
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Cant edit this comment'

		RETURN
	END


	UPDATE dbo.PRFeedback
	SET [Content] = @Content, ModifiedDate = @Today
	WHERE PRFeedbackId = @PRFeedbackId
		AND CreatorId = @CreatorId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PRFeedbackId AS '@id'
				,@CreatorId AS '@cid'
				,@OldContent AS '@old_content'
				,@Content AS '@new_content'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedback_Update'
		,@Data = @logData
		,@Duration = @duration
END