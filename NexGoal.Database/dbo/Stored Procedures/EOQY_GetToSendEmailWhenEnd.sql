

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

	DECLARE @ManagementType TINYINT

	SET @ManagementType = 2

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,DATEADD(day,1,EndDate) AS StartDate
		,DATEADD(day,9999,EndDate) AS EndDate
		,RemindDate
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE DATEADD(day, 1, EndDate) = CAST(GETDATE() AS DATE)
	ORDER BY [Year]
		,[Quarter] DESC

	SELECT eoqy.EOQYId
		,setting.RoleId
		,e.NickName
		,e.Email
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON setting.EOQYId = eoqy.EOQYId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = setting.NominatorId
	WHERE DATEADD(day, 1, eoqy.EndDate) = CAST(GETDATE() AS DATE)
		AND setting.RoleId = @ManagementType
	ORDER BY [Year]
		,[Quarter] DESC
END