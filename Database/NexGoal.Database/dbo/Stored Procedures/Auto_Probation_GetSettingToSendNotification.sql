CREATE PROCEDURE [dbo].[Auto_Probation_GetSettingToSendNotification] @ReceiveType TINYINT
AS
/*
Created: 20190110@Tim.Trinh
Task: get setting to send notification.
DB: bodb_NexGoal
Original:

Revisions:
Oliver.Dang revised, get skype message settings
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@ProbationDuration SMALLINT = 2 -- month
		,@RemindDay SMALLINT
		,@HRRemindDay SMALLINT
		,@EvaluatorRemindDay SMALLINT
		,@ReviewerRemindDay SMALLINT
		,@NotificationType TINYINT = 1
		,@RemindMode TINYINT = 2
		,@HRRoleId TINYINT = 2
		-- process
		,@HRProcessId TINYINT = 1
		,@SelfProcessId TINYINT = 2
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		-- receive type
		,@ReceiveMail TINYINT = 1
		,@ReceiveNotification TINYINT = 2
		,@ReceiveSkypeMessage TINYINT = 3

	CREATE TABLE #tblProcessSetting (
		ProcessId TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		,ConversationId VARCHAR(255)
		,EndDate DATE
		,ProbationerNickname NVARCHAR(20)
		)

	SELECT @RemindDay = CAST(Value AS SMALLINT)
		,@HRRemindDay = CAST(Value AS SMALLINT) + 2
		,@EvaluatorRemindDay = CAST(Value AS SMALLINT) - 2
		,@ReviewerRemindDay = CAST(Value AS SMALLINT) - 4
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationRemindDay'

	INSERT INTO #tblProcessSetting (
		ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,EndDate
		)
	SELECT @SelfProcessId
		,EmployeeId
		,NickName
		,FullName
		,Email
		,ConversationId
		,DATEADD(MONTH, @ProbationDuration, JoinedDate) AS EndDate
	FROM dbo.Employee WITH (NOLOCK)
	WHERE [Enabled] = 1
		AND IsDeleted = 0
		AND DATEADD(DAY, - @RemindDay, DATEADD(MONTH, @ProbationDuration, JoinedDate)) = @Today

	INSERT INTO #tblProcessSetting (
		ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,EndDate
		,ProbationerNickname
		)
	SELECT @HRProcessId
		,hr.EmployeeId
		,hr.NickName
		,hr.FullName
		,hr.Email
		,hr.ConversationId
		,DATEADD(MONTH, @ProbationDuration, member.JoinedDate) AS EndDate
		,member.NickName
	FROM dbo.Employee AS hr WITH (NOLOCK)
		,dbo.Employee AS member WITH (NOLOCK)
	WHERE hr.[Enabled] = 1
		AND hr.IsDeleted = 0
		AND member.[Enabled] = 1
		AND member.IsDeleted = 0
		AND DATEADD(DAY, - @HRRemindDay, DATEADD(MONTH, @ProbationDuration, member.JoinedDate)) = @Today
		AND EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
			WHERE hr.EmployeeId = erole.EmployeeId
				AND erole.RoleId = @HRRoleId
			)

	INSERT INTO #tblProcessSetting (
		ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,EndDate
		,ProbationerNickname
		)
	SELECT @EvaluatorProcessId
		,efs.EvaluatorId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
		,DATEADD(MONTH, @ProbationDuration, member.JoinedDate)
		,member.NickName
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON efs.EvaluatorId = evaluator.EmployeeId
		AND evaluator.[Enabled] = 1
		AND evaluator.IsDeleted = 0
		INNER JOIN dbo.Employee AS member WITH (NOLOCK) ON efs.EmployeeId = member.EmployeeId
		AND member.[Enabled] = 1
		AND member.IsDeleted = 0
	WHERE (
			efs.ProcessId = @EvaluatorProcessId
			AND DATEADD(DAY, - @EvaluatorRemindDay, DATEADD(MONTH, @ProbationDuration, member.JoinedDate)) = @Today
			)
		OR (
			efs.ProcessId = @ReviewerProcessId
			AND DATEADD(DAY, - @ReviewerRemindDay, DATEADD(MONTH, @ProbationDuration, member.JoinedDate)) = @Today
			)

	INSERT INTO #tblProcessSetting (
		ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,EndDate
		,ProbationerNickname
		)
	SELECT @EvaluatorProcessId
		,gfs.EvaluatorId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
		,DATEADD(MONTH, @ProbationDuration, member.JoinedDate)
		,member.NickName
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON gfs.EvaluatorId = evaluator.EmployeeId
		AND evaluator.[Enabled] = 1
		AND evaluator.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		INNER JOIN dbo.Employee AS member WITH (NOLOCK) ON gr.GroupId = member.GroupId
		AND member.[Enabled] = 1
		AND member.IsDeleted = 0
	WHERE (
			(
				gfs.ProcessId = @EvaluatorProcessId
				AND DATEADD(DAY, - @EvaluatorRemindDay, DATEADD(MONTH, @ProbationDuration, member.JoinedDate)) = @Today
				)
			OR (
				gfs.ProcessId = @ReviewerProcessId
				AND DATEADD(DAY, - @ReviewerRemindDay, DATEADD(MONTH, @ProbationDuration, member.JoinedDate)) = @Today
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
			WHERE member.EmployeeId = efs.EmployeeId
			);

	SELECT ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,EndDate
		,ConversationId
		,ProbationerNickname
	FROM #tblProcessSetting AS tbl WITH (NOLOCK)
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

	DROP TABLE #tblProcessSetting
END
