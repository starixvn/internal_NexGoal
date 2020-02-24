CREATE PROCEDURE [dbo].[PRFeedbackReaction_Insert] @PRFeedbackId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Agree PR's feedback and send notification
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @RoleId TINYINT = 0
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	DECLARE @Today DATETIME = GETDATE()
		,@PerformanceRecordId INT
		,@EmployeeId INT
		,@FeedbackCreatorId INT
		,@CreatorNickName NVARCHAR(20)
		,@NotiStatus_Unread TINYINT = 0
		,@NotiAction TINYINT = 4 -- Member page with special performance checkin
		,@NotiContent NVARCHAR(MAX) = ' likes your comment'

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SELECT @EmployeeId = pr.EmployeeId, @FeedbackCreatorId = fb.CreatorId, @PerformanceRecordId = fb.PerformanceRecordId
	FROM PRFeedback AS fb WITH (NOLOCK)
	INNER JOIN PerformanceRecord AS pr WITH (NOLOCK) ON fb.PerformanceRecordId = pr.PerformanceRecordId
	WHERE fb.PRFeedbackId = @PRFeedbackId

	IF EXISTS (
		SELECT 1
		FROM dbo.PRFeedbackReaction WITH (NOLOCK)
		WHERE PRFeedbackId = @PRFeedbackId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Only one reaction is allowed per user'

		RETURN
	END

	SELECT @RoleId = CASE
			WHEN MAX(data.[Priority]) = 2 THEN @ReviewerProcessId
			WHEN MAX(data.[Priority]) = 3 THEN @EvaluatorProcessId
			ELSE @PMProcessId  
		END
	FROM
	(SELECT CASE
			WHEN ProcessId = @EvaluatorProcessId THEN 3
			WHEN ProcessId = @ReviewerProcessId THEN 2
			WHEN ProcessId = @PMProcessId THEN 1
			ELSE 0
		END AS [Priority]
	FROM dbo.func_GetMember(@CreatorId)
	WHERE EmployeeId = @EmployeeId) AS data

	IF (@CreatorId = @EmployeeId OR @RoleId = 0)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Permission denied'
		
		RETURN
	END

	SELECT @CreatorNickName = NickName
	FROM Employee WITH (NOLOCK)
	WHERE EmployeeId = @CreatorId

	INSERT INTO PRFeedbackReaction (PRFeedbackId, CreatorId, RoleId, CreatedDate)
		VALUES (@PRFeedbackId, @CreatorId, @RoleId, @Today)

	INSERT INTO [Notification] (RecordId, ActionTypeId, StatusId, CreatorId, EmployeeId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @NotiAction, @NotiStatus_Unread, @CreatorId, @FeedbackCreatorId, @CreatorNickName + @NotiContent, @Today)

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PRFeedbackId AS '@ids'
				,@EmployeeId AS '@eid'
				,@CreatorId AS '@cid'
				,@RoleId AS '@role'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedbackReaction_Insert'
		,@Data = @logData
		,@Duration = @duration
END