CREATE PROCEDURE [dbo].[PAFocus_GetProcessSettingToSendEmail]
AS
/*
Created: 20171004@Tim.Trinh
Task: get process setting to send email
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@StartMode TINYINT = 1
		,@ReminderMode TINYINT = 2

	SELECT DISTINCT @StartMode AS MailMode
		,f.PeriodId
		,p.[Name] AS PeriodName
		,pc.EvaluatorId
		,e.NickName
		,e.FullName
		,e.Email
		,pc.StartDate
		,pc.EndDate
	FROM dbo.PAFocusProcess AS pc WITH (NOLOCK)
		INNER JOIN dbo.PAFocus AS f WITH (NOLOCK) ON f.PAFocusId = pc.PAFocusId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON f.PeriodId = p.PeriodId
		AND p.IsActive = 1
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pc.EvaluatorId = e.EmployeeId
		AND e.[Enabled] = 1
		AND e.IsDeleted = 0
	WHERE pc.ProcessId = 3
		AND pc.StartDate = @Today
	
	UNION
	SELECT DISTINCT @ReminderMode AS MailMode
		,f.PeriodId
		,p.[Name] AS PeriodName
		,pc.EvaluatorId
		,e.NickName
		,e.FullName
		,e.Email
		,pc.StartDate
		,pc.EndDate
	FROM dbo.PAFocusProcess AS pc WITH (NOLOCK)
		INNER JOIN dbo.PAFocus AS f WITH (NOLOCK) ON f.PAFocusId = pc.PAFocusId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON f.PeriodId = p.PeriodId
		AND p.IsActive = 1
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pc.EvaluatorId = e.EmployeeId
		AND e.[Enabled] = 1
		AND e.IsDeleted = 0
	WHERE pc.ProcessId = 3
		AND CAST(DATEADD(DAY, - 1, pc.EndDate) AS DATE) = @Today
END