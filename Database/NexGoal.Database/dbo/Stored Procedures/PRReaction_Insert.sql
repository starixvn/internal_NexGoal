CREATE PROCEDURE [dbo].[PRReaction_Insert]  @PerformanceRecordId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Agree member's PR and send notificaion
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
	 ,@ReactionCreatorNickName NVARCHAR(20)
	 ,@PRCreatorId INT
	 ,@NotiStatus_Unread TINYINT = 0
	 ,@NotiAction_MyPage TINYINT = 2 -- My page with special performance checkin
	 ,@NotiAction_MemberPage TINYINT = 4 -- My page with special performance checkin
	 ,@NotiAction TINYINT
	 ,@NotiContent NVARCHAR(MAX) = ' likes your Performance Record '

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SELECT @EmployeeId = pr.EmployeeId, @PRCreatorId = pr.CreatorId,
		@NotiAction = CASE
			WHEN pr.RoleId = 2 THEN @NotiAction_MyPage
			ELSE @NotiAction_MemberPage
		END
	FROM PerformanceRecord pr WITH (NOLOCK)
	WHERE pr.PerformanceRecordId = @PerformanceRecordId

	IF EXISTS (
		SELECT 1
		FROM dbo.PRReaction WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Only one reaction is allowed per user'

		RETURN
	END

	IF EXISTS (
		SELECT 1
		FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		WHERE pr.PerformanceRecordId = @PerformanceRecordId
			AND pr.CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Can not agree your PR'

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

	SELECT @ReactionCreatorNickName = NickName
	FROM Employee WITH (NOLOCK)
	WHERE EmployeeId = @CreatorId

	IF(@NotiAction = @NotiAction_MyPage)
	BEGIN
		SET @NotiContent = @ReactionCreatorNickName + @NotiContent
	END
	ELSE
	BEGIN
		DECLARE @EmployeeNickName NVARCHAR(20)

		SELECT @EmployeeNickName = NickName
		FROM Employee WITH (NOLOCK)
		WHERE EmployeeId = @EmployeeId

		SET @NotiContent = @ReactionCreatorNickName + @NotiContent + 'for ' + @EmployeeNickName
	END

	INSERT INTO PRReaction (PerformanceRecordId, CreatorId, RoleId, CreatedDate)
		VALUES (@PerformanceRecordId, @CreatorId, @RoleId, @Today)

	INSERT INTO [Notification] (RecordId, ActionTypeId, StatusId, CreatorId, EmployeeId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @NotiAction, @NotiStatus_Unread, @CreatorId, @PRCreatorId, @NotiContent , @Today)

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordId AS '@ids'
				,@EmployeeId AS '@eid'
				,@CreatorId AS '@cid'
				,@RoleId AS '@role'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRReaction_Insert'
		,@Data = @logData
		,@Duration = @duration
END