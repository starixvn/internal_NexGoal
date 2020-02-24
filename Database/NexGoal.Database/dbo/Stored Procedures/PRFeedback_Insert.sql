CREATE PROCEDURE [dbo].[PRFeedback_Insert] @PerformanceRecordId INT
	,@CreatorId INT
	,@Content NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Comment on member's PR and send notification
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
		,@EmployeeId INT
		,@CreatorNickName NVARCHAR(20)
		 ,@NotiStatus_Unread TINYINT = 0
		 ,@NotiAction TINYINT = 2 -- My page with special performance checkin
		 ,@NotiContent NVARCHAR(MAX) = ' commented on your performance record'

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SELECT @EmployeeId = pr.EmployeeId
	FROM PerformanceRecord pr
	WHERE pr.PerformanceRecordId = @PerformanceRecordId

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PerformanceRecord WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND TypeId = 1 -- performance checkin
			AND RoleId = 2 -- self checkin
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Can not comment on this PR'

		RETURN
	END

	IF EXISTS (
		SELECT 1
		FROM dbo.PRFeedback WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Only one comment is allowed per user'

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

	INSERT INTO dbo.PRFeedback(PerformanceRecordId, CreatorId, RoleId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @CreatorId, @RoleId, @Content, @Today)

	INSERT INTO [Notification] (RecordId, ActionTypeId, StatusId, CreatorId, EmployeeId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @NotiAction, @NotiStatus_Unread, @CreatorId, @EmployeeId, @CreatorNickName + @NotiContent, @Today)

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordId AS '@id'
				,@EmployeeId AS '@eid'
				,@CreatorId AS '@cid'
				,@RoleId AS '@role'
				,@Content AS '@content'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedback_Insert'
		,@Data = @logData
		,@Duration = @duration
END