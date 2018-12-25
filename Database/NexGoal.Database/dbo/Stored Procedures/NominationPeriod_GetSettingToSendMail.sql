
CREATE PROCEDURE [dbo].[NominationPeriod_GetSettingToSendMail]
AS
/*
Created: 20181224@Tim.Trinh
Task: get setting to send mail.
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
		,@MailType TINYINT = 3

	CREATE TABLE #tblProcessSetting (
		NominationPeriodId INT
		,MailMode TINYINT
		,NominatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		)

	INSERT INTO #tblProcessSetting (
		NominationPeriodId
		,MailMode
		,NominatorId
		,Nickname
		,FullName
		,Email
		)
	SELECT period.EOQYId AS NominationPeriodId
		,(CASE 
			WHEN period.StartDateForNominator = @Today THEN @StartMode
			ELSE @RemindMode END
			) AS MailMode
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
				LEFT JOIN dbo.MailConfig AS mail WITH (NOLOCK) ON tbl.NominatorId = mail.EmployeeId
			WHERE period.EOQYId = tbl.NominationPeriodId
				AND ISNULL(mail.[Type], @MailType) = @MailType
				AND ISNULL(mail.Notify, 1) = 1
			)

	SELECT NominationPeriodId
		,MailMode
		,NominatorId
		,Nickname
		,FullName
		,Email
	FROM #tblProcessSetting AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.MailConfig AS mail WITH (NOLOCK) ON tbl.NominatorId = mail.EmployeeId
	WHERE ISNULL(mail.[Type], @MailType) = @MailType
		AND ISNULL(mail.Notify, 1) = 1

	DROP TABLE #tblProcessSetting
END