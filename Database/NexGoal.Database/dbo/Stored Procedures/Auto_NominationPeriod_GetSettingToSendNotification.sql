
CREATE PROCEDURE [dbo].[Auto_NominationPeriod_GetSettingToSendNotification] @ReceiveType TINYINT
AS
/*
Created: 20190107@Tim.Trinh
Task: get setting to send notification.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@StartMode TINYINT = 1
		,@RemindMode TINYINT = 2
		,@NotificationType TINYINT = 3
		-- receive type
		,@ReceiveMail TINYINT = 1
		,@ReceiveNotification TINYINT = 2

	CREATE TABLE #tblProcessSetting (
		NominationPeriodId INT
		,NotificationMode TINYINT
		,NominatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		)

	INSERT INTO #tblProcessSetting (
		NominationPeriodId
		,NotificationMode
		,NominatorId
		,Nickname
		,FullName
		,Email
		)
	SELECT period.EOQYId AS NominationPeriodId
		,(CASE 
			WHEN period.StartDateForNominator = @Today THEN @StartMode
			ELSE @RemindMode END
			) AS NotificationMode
		,setting.NominatorId
		,emp.NickName
		,emp.FullName
		,emp.Email
	FROM dbo.EOQY AS period WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON period.EOQYId = setting.EOQYId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON setting.NominatorId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		CROSS APPLY dbo.func_SplitString(setting.RoleIds, ',') AS f
	WHERE (
			period.StartDateForNominator = @Today
			OR period.RemindDateForNominator = @Today
			)
		AND f.val = 1

	SELECT EOQYId AS NominationPeriodId
		,[Quarter]
		,[Year]
		,StartDateForNominator
		,EndDateForNominator
	FROM dbo.EOQY AS period WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #tblProcessSetting AS tbl WITH (NOLOCK)
				LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.NominatorId = setting.EmployeeId
			WHERE period.EOQYId = tbl.NominationPeriodId
				AND ISNULL(setting.TypeId, @NotificationType) = @NotificationType
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
			)

	SELECT NominationPeriodId
		,NotificationMode
		,NominatorId
		,Nickname
		,FullName
		,Email
	FROM #tblProcessSetting AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.NominatorId = setting.EmployeeId
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