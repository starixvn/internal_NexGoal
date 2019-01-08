
CREATE PROCEDURE [dbo].[Auto_PAFocus_InsertNotification]
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
		,@NotificationType TINYINT = 4

	CREATE TABLE #tblProcessSetting (
		PeriodId INT
		,PeriodName NVARCHAR(200)
		,FocusStartDate DATE
		,FocusEndDate Date
		,NotificationMode TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		)

	INSERT INTO #tblProcessSetting (
		PeriodId
		,PeriodName
		,FocusStartDate
		,FocusEndDate
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
		)
	SELECT DISTINCT period.PeriodId
		,period.[Name]
		,period.FocusStartDate
		,period.FocusEndDate
		,(CASE 
			WHEN period.FocusStartDate = @Today THEN @StartMode
			ELSE @RemindMode END
			) AS NotificationMode
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
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
		,NotificationMode
		,EvaluatorId
		,Nickname
		,FullName
	FROM #tblProcessSetting AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.EvaluatorId = setting.EmployeeId
	WHERE ISNULL(setting.TypeId, @NotificationType) = @NotificationType
		AND ISNULL(setting.ReceiveNotification, 1) = 1

	DROP TABLE #tblProcessSetting
END