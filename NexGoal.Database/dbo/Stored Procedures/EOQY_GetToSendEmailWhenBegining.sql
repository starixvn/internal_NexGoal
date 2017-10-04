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

	DECLARE @Today DATE = GETDATE()

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE StartDate = @Today
	ORDER BY [Year]
		,[Quarter] DESC

	SELECT eoqy.EOQYId
		,e.NickName
		,e.Email
		,f.val AS RoleId
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON setting.EOQYId = eoqy.EOQYId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = setting.NominatorId
		CROSS APPLY dbo.fnc_strSplit(setting.RoleId, ',') AS f
	WHERE eoqy.StartDate = @Today
	ORDER BY [Year]
		,[Quarter] DESC
END