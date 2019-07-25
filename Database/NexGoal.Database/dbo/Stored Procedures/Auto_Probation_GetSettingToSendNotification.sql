CREATE PROCEDURE [dbo].[Auto_Probation_GetSettingToSendNotification] @ReceiveType TINYINT
AS
/*
Created: 20190110@Tim.Trinh
Task: get setting to send notification.
DB: bodb_NexGoal
Original:

Revisions:
Oliver.Dang revised, get skype message settings
20190725@Elton.Ton: change with new probation function
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@EvaluatorRemindDay SMALLINT
		,@NotificationType TINYINT = 5
		,@RemindMode TINYINT = 2
		,@StartMode TINYINT = 1
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
		,NotificationMode TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		,ConversationId VARCHAR(255)
		,StartDate DATE
		,EndDate DATE
		,ProbationerNickname NVARCHAR(20)
		)

	SELECT @EvaluatorRemindDay = CAST(Value AS SMALLINT) - 2
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationRemindDay'

	INSERT INTO #tblProcessSetting (
		ProcessId
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,StartDate
		,ProbationerNickname
		)
	SELECT @HRProcessId
		,@StartMode
		,hr.EmployeeId
		,hr.NickName
		,hr.FullName
		,hr.Email
		,hr.ConversationId
		,member.JoinedDate
		,member.NickName
	FROM dbo.Employee AS hr WITH (NOLOCK)
		,dbo.Employee AS member WITH (NOLOCK)
	WHERE hr.[Enabled] = 1
		AND hr.IsDeleted = 0
		AND member.[Enabled] = 1
		AND member.IsDeleted = 0
		AND member.JoinedDate = @Today
		AND EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
				WHERE hr.EmployeeId = erole.EmployeeId
					AND erole.RoleId = @HRRoleId
			)

	INSERT INTO #tblProcessSetting (
		ProcessId
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,StartDate
		,EndDate
		)
	SELECT @SelfProcessId
		,@StartMode
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,e.Email
		,e.ConversationId
		,ps.FromDate
		,ps.ToDate
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN dbo.ProbationSetting ps WITH (NOLOCK) ON e.EmployeeId = ps.EmployeeId
	WHERE [Enabled] = 1
		AND IsDeleted = 0
		AND ps.FromDate = @Today
		AND ps.ProcessId = @SelfProcessId

	
	INSERT INTO #tblProcessSetting (
		ProcessId
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,StartDate
		,EndDate
		,ProbationerNickname
		)
	SELECT @EvaluatorProcessId
		,@StartMode
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
		,ps.FromDate
		,ps.ToDate
		,member.NickName
	FROM dbo.ProbationSetting ps WITH (NOLOCK)
		INNER JOIN dbo.Employee evaluator ON ps.EvaluatorId = evaluator.EmployeeId
		INNER JOIN dbo.Employee member ON ps.EmployeeId = member.EmployeeId
	WHERE ps.ProcessId = @EvaluatorProcessId AND ps.FromDate = @Today

	INSERT INTO #tblProcessSetting (
		ProcessId
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,StartDate
		,EndDate
		,ProbationerNickname
		)
	SELECT @EvaluatorProcessId
		,@RemindMode
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
		,ps.FromDate
		,ps.ToDate
		,member.NickName
	FROM dbo.ProbationSetting ps WITH (NOLOCK)
		INNER JOIN dbo.Employee evaluator ON ps.EvaluatorId = evaluator.EmployeeId
		INNER JOIN dbo.Employee member ON ps.EmployeeId = member.EmployeeId
	WHERE ps.ProcessId = @EvaluatorProcessId AND ps.ToDate = @Today

	INSERT INTO #tblProcessSetting (
		ProcessId
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,StartDate
		,EndDate
		,ProbationerNickname
		)
	SELECT @ReviewerProcessId
		,@StartMode
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,evaluator.ConversationId
		,ps.FromDate
		,ps.ToDate
		,member.NickName
	FROM dbo.ProbationSetting ps WITH (NOLOCK)
		INNER JOIN dbo.Employee evaluator ON ps.EvaluatorId = evaluator.EmployeeId
		INNER JOIN dbo.Employee member ON ps.EmployeeId = member.EmployeeId
	WHERE ps.ProcessId = @ReviewerProcessId AND ps.FromDate = @Today

	SELECT ProcessId
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,StartDate
		,EndDate
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