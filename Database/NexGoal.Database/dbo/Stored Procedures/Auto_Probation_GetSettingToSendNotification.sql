
CREATE PROCEDURE [dbo].[Auto_Probation_GetSettingToSendNotification] @ReceiveType TINYINT
AS
/*
Created: 20190110@Tim.Trinh
Task: get setting to send notification.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@ProbationDuration SMALLINT
		,@ProbationRemindDay SMALLINT
		,@NotificationType TINYINT = 1
		,@RemindMode TINYINT = 2
		,@SelfProcessId TINYINT = 2
		,@EvaluatorProcessId TINYINT = 4
		-- receive type
		,@ReceiveMail TINYINT = 1
		,@ReceiveNotification TINYINT = 2

	CREATE TABLE #tblProcessSetting (
		ProcessId TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		,EndDate DATE
		)

	SELECT TOP 1 @ProbationDuration = CAST(Value AS SMALLINT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationDuration'

	SELECT TOP 1 @ProbationRemindDay = CAST(Value AS SMALLINT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationRemindDay'

	INSERT INTO #tblProcessSetting (
		ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,EndDate
		)
	SELECT @SelfProcessId
		,EmployeeId
		,NickName
		,FullName
		,Email
		,DATEADD(DAY, @ProbationDuration, JoinedDate) AS EndDate
	FROM dbo.Employee WITH (NOLOCK)
	WHERE [Enabled] = 1
		AND IsDeleted = 0
		AND DATEADD(DAY, - @ProbationRemindDay, DATEADD(DAY, @ProbationDuration, JoinedDate)) = @Today

	INSERT INTO #tblProcessSetting (
		ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,EndDate
		)
	SELECT @EvaluatorProcessId
		,efs.EvaluatorId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,member.EndDate
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON efs.EvaluatorId = evaluator.EmployeeId
		AND evaluator.[Enabled] = 1
		AND evaluator.IsDeleted = 0
		INNER JOIN #tblProcessSetting AS member WITH (NOLOCK) ON efs.EmployeeId = member.EvaluatorId
		AND member.ProcessId = @SelfProcessId
	WHERE efs.ProcessId = @EvaluatorProcessId
	
	UNION
	SELECT @EvaluatorProcessId
		,gfs.EvaluatorId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,tblsetting.EndDate
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON gfs.EvaluatorId = evaluator.EmployeeId
		AND evaluator.[Enabled] = 1
		AND evaluator.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		INNER JOIN dbo.Employee AS member WITH (NOLOCK) ON gr.GroupId = member.GroupId
		INNER JOIN #tblProcessSetting AS tblsetting WITH (NOLOCK) ON member.EmployeeId = tblsetting.EvaluatorId
		AND tblsetting.ProcessId = @SelfProcessId
	WHERE gfs.ProcessId = @EvaluatorProcessId

	SELECT ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,EndDate
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
			)

	DROP TABLE #tblProcessSetting
END