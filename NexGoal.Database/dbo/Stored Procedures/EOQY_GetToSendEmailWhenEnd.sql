CREATE PROCEDURE [dbo].[EOQY_GetToSendEmailWhenEnd]
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

	DECLARE @ManagementType TINYINT = 2
		,@Today DATE = GETDATE()

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,DATEADD(day, 1, EndDate) AS StartDate
		,EndDateForCommittee AS EndDate
		,RemindDate
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE DATEADD(day, 1, EndDate) = @Today
	ORDER BY [Year]
		,[Quarter] DESC

	SELECT eoqy.EOQYId
		,f.val AS RoleId
		,e.NickName
		,e.Email
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON setting.EOQYId = eoqy.EOQYId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = setting.NominatorId
		CROSS APPLY dbo.fnc_strSplit(setting.RoleId, ',') AS f
	WHERE DATEADD(day, 1, eoqy.EndDate) = @Today
		AND f.val = @ManagementType
	ORDER BY [Year]
		,[Quarter] DESC
END