
CREATE PROCEDURE [dbo].[PAFocus_GetSettingToSendMail]
AS
/*
Created: 20181221@Tim.Trinh
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
		,@EvaluatorProcessId TINYINT = 4
		,@MailType TINYINT = 4

	CREATE TABLE #tblProcessSetting (
		PeriodId INT
		,MailMode TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		)

	INSERT INTO #tblProcessSetting (
		PeriodId
		,MailMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		)
	SELECT period.PeriodId
		,(CASE 
			WHEN period.FocusStartDate = @Today THEN @StartMode
			ELSE @RemindMode END
			) AS MailMode
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Email
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	WHERE (
			period.FocusStartDate = @Today
			OR DATEADD(DAY, - 2, period.FocusEndDate) = @Today
			)
		AND period.IsActive = 1

	SELECT PeriodId
		,[Name]
		,FocusStartDate
		,FocusEndDate
	FROM dbo.Period AS period WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #tblProcessSetting AS tbl WITH (NOLOCK)
				LEFT JOIN dbo.MailConfig AS mail WITH (NOLOCK) ON tbl.EvaluatorId = mail.EmployeeId
			WHERE period.PeriodId = tbl.PeriodId
				AND ISNULL(mail.[Type], @MailType) = @MailType
				AND ISNULL(mail.Notify, 1) = 1
			)

	SELECT PeriodId
		,MailMode
		,EvaluatorId
		,Nickname
		,FullName
		,Email
	FROM #tblProcessSetting AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.MailConfig AS mail WITH (NOLOCK) ON tbl.EvaluatorId = mail.EmployeeId
	WHERE ISNULL(mail.[Type], @MailType) = @MailType
		AND ISNULL(mail.Notify, 1) = 1

	DROP TABLE #tblProcessSetting
END