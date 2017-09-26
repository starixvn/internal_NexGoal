

CREATE PROCEDURE [dbo].[EOQY_GetToSendEmailWhenBegining]
AS
/*
Created: 20161229@Razor.Nguyen
Task: Remind Date.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE StartDate = CAST(GETDATE() AS DATE)
	ORDER BY [Year]
		,[Quarter] DESC

	SELECT eoqy.EOQYId
		,e.NickName
		,e.Email
		,setting.RoleId
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH(NOLOCK) ON setting.EOQYId = eoqy.EOQYId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = setting.NominatorId
	WHERE eoqy.StartDate = CAST(GETDATE() AS DATE)
	ORDER BY [Year]
		,[Quarter] DESC
END