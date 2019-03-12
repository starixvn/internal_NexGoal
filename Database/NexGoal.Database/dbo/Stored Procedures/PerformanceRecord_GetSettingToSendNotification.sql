
CREATE PROCEDURE [dbo].[PerformanceRecord_GetSettingToSendNotification] @ReceiveType TINYINT
	,@PerformanceRecordIds VARCHAR(MAX)
	,@ActorId INT
AS
/*
Created: 20181127@Tim.Trinh
Task: Get seting to send notification.
DB: bodb_NexGoal
Original:

Revisions: Oliver.Dang
Add ReceiveType by skype chat bot
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@NotificationType TINYINT = 1
		,@PeriodId INT
		,@PARejected TINYINT = 5
		-- mail mode
		,@MemberToSupervisor TINYINT = 1
		,@SupervisorToMember TINYINT = 2
		,@SupervisorToSupervisor TINYINT = 3
		-- receive type
		,@ReceiveMail TINYINT = 1
		,@ReceiveNotification TINYINT = 2
		,@ReceiveSkypeMessage TINYINT = 3
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	CREATE TABLE #tblPerformanceRecordIds (
		PerformanceRecordId INT
		,EmployeeId INT
		,RoleId TINYINT
		)

	CREATE TABLE #tblResult (
		PerformanceRecordId INT
		,NotificationMode TINYINT
		,ProcessId TINYINT
		,[Priority] TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		,ConversationId VARCHAR(255)
		)

	SELECT TOP 1 @PeriodId = PeriodId
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND @Today BETWEEN DisciplineStartDate
			AND DisciplineEndDate
	ORDER BY DisciplineStartDate DESC

	INSERT INTO #tblPerformanceRecordIds (
		PerformanceRecordId
		,EmployeeId
		,RoleId
		)
	SELECT val
		,record.EmployeeId
		,record.RoleId
	FROM dbo.func_SplitString(@PerformanceRecordIds, ',') AS tbl
		INNER JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON ISNUMERIC(tbl.val) = 1
		AND tbl.val = record.PerformanceRecordId

	-- member
	INSERT INTO #tblResult (
		PerformanceRecordId
		,NotificationMode
		,ProcessId
		,[Priority]
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		)
	SELECT tbl.PerformanceRecordId
		,@SupervisorToMember AS NotificationMode
		,@SelfProcessId AS ProcessId
		,1 AS [Priority]
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.ConversationId
	FROM #tblPerformanceRecordIds AS tbl WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON tbl.EmployeeId = emp.EmployeeId
	WHERE tbl.EmployeeId <> @ActorId

	-- evaluator, reviewer
	INSERT INTO #tblResult (
		PerformanceRecordId
		,NotificationMode
		,ProcessId
		,[Priority]
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		)
	SELECT record.PerformanceRecordId
		,(CASE record.RoleId
			WHEN @SelfProcessId THEN @MemberToSupervisor
			ELSE @SupervisorToSupervisor END
			) AS NotificationMode
		,process.ProcessId
		,(CASE process.ProcessId
			WHEN @EvaluatorProcessId THEN 2
			ELSE 3 END
			) AS [Priority]
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
		AND process.EvaluatorId <> @ActorId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		INNER JOIN #tblPerformanceRecordIds AS record WITH (NOLOCK) ON pa.EmployeeId = record.EmployeeId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON process.EvaluatorId = evaluator.EmployeeId
	WHERE pa.StatusId <> @PARejected
		AND NOT EXISTS (
			SELECT 1
			FROM #tblResult AS tbl WITH (NOLOCK)
			WHERE record.PerformanceRecordId = tbl.PerformanceRecordId
				AND process.ProcessId = tbl.ProcessId
			)

	INSERT INTO #tblResult (
		PerformanceRecordId
		,NotificationMode
		,ProcessId
		,[Priority]
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		)
	SELECT record.PerformanceRecordId
		,(CASE record.RoleId
			WHEN @SelfProcessId THEN @MemberToSupervisor
			ELSE @SupervisorToSupervisor END
			) AS NotificationMode
		,efs.ProcessId
		,(CASE efs.ProcessId
			WHEN @EvaluatorProcessId THEN 2
			ELSE 3 END
			) AS [Priority]
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN #tblPerformanceRecordIds AS record WITH (NOLOCK) ON efs.EmployeeId = record.EmployeeId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON efs.EvaluatorId = evaluator.EmployeeId
		LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON pa.EmployeeId = efs.EmployeeId
		AND pa.PeriodId = @PeriodId
		AND pa.StatusId <> @PARejected
	WHERE efs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
		AND efs.EvaluatorId <> @ActorId
		AND pa.PARecordId IS NULL
		AND NOT EXISTS (
			SELECT 1
			FROM #tblResult AS tbl WITH (NOLOCK)
			WHERE record.PerformanceRecordId = tbl.PerformanceRecordId
				AND efs.ProcessId = tbl.ProcessId
			)

	INSERT INTO #tblResult (
		PerformanceRecordId
		,NotificationMode
		,ProcessId
		,[Priority]
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		)
	SELECT record.PerformanceRecordId
		,(CASE record.RoleId
			WHEN @SelfProcessId THEN @MemberToSupervisor
			ELSE @SupervisorToSupervisor END
			) AS NotificationMode
		,gfs.ProcessId
		,(CASE gfs.ProcessId
			WHEN @EvaluatorProcessId THEN 2
			ELSE 3 END
			) AS [Priority]
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
		INNER JOIN #tblPerformanceRecordIds AS record WITH (NOLOCK) ON emp.EmployeeId = record.EmployeeId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON gfs.EvaluatorId = evaluator.EmployeeId
		LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		AND pa.PeriodId = @PeriodId
		AND pa.StatusId <> @PARejected
	WHERE gfs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
		AND gfs.EvaluatorId <> @ActorId
		AND pa.PARecordId IS NULL
		AND NOT EXISTS (
			SELECT 1
			FROM #tblResult AS tbl WITH (NOLOCK)
			WHERE record.PerformanceRecordId = tbl.PerformanceRecordId
				AND gfs.ProcessId = tbl.ProcessId
			)

	-- project manager
	INSERT INTO #tblResult (
		PerformanceRecordId
		,NotificationMode
		,ProcessId
		,[Priority]
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		)
	SELECT record.PerformanceRecordId
		,@MemberToSupervisor AS NotificationMode
		,@PMProcessId AS ProcessId
		,4 AS [Priority]
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.ConversationId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		,#tblPerformanceRecordIds AS record WITH (NOLOCK)
	WHERE emp.EmployeeId <> @ActorId
		AND record.RoleId = @SelfProcessId
		AND EXISTS (
			SELECT 1
			FROM dbo.PMSetting AS pmsetting WITH (NOLOCK)
			WHERE emp.EmployeeId = pmsetting.ProjectManagerId
				AND record.EmployeeId = pmsetting.MemberId
			)

	SELECT tbl.PerformanceRecordId
		,emp.EmployeeId
		,emp.NickName
		,emp.Email
	FROM #tblPerformanceRecordIds AS tbl WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON tbl.EmployeeId = emp.EmployeeId

	SELECT DISTINCT tbl.PerformanceRecordId
		,tbl.NotificationMode
		,tbl.ProcessId
		,tbl.[Priority]
		,tbl.EvaluatorId
		,tbl.Nickname
		,tbl.FullName
		,tbl.Email
		,tbl.ConversationId
	FROM #tblResult AS tbl WITH (NOLOCK)
		INNER JOIN (
		SELECT PerformanceRecordId
			,EvaluatorId
			,MIN([Priority]) AS [Priority]
		FROM #tblResult WITH (NOLOCK)
		GROUP BY PerformanceRecordId
			,EvaluatorId
		) AS data ON tbl.PerformanceRecordId = data.PerformanceRecordId
		AND data.EvaluatorId = data.EvaluatorId
		AND tbl.[Priority] = data.[Priority]
		LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.EvaluatorId = setting.EmployeeId
	WHERE ISNULL(setting.TypeId, @NotificationType) = @NotificationType
		AND (
			(
				@ReceiveType = @ReceiveMail
				AND ISNULL(setting.ReceiveEmail, 1) = 1
				)
			OR (
				@ReceiveType = @ReceiveNotification
				AND ISNULL(setting.ReceiveNotification, 1) = 1
				)
			OR (
				@ReceiveType = @ReceiveSkypeMessage
				AND ISNULL(setting.ReceiveSkypeMessage, 1) = 1
				)
			)
	ORDER BY tbl.PerformanceRecordId
		,tbl.ProcessId

	DROP TABLE #tblPerformanceRecordIds

	DROP TABLE #tblResult
END