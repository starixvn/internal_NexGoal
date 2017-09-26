


CREATE PROCEDURE [dbo].[EOY_GetLatestNominationByNominatorId] @NominatorId INT
AS
/*
Created: 20160912@Tim.Trinh
Task: Get current nomination by NominatorId.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 1 setting.RoleId
		,eoq.EOQYId
		,eoq.[Quarter]
		,eoq.[Year]
		,eoq.StartDate
		,eoq.EndDate
		,eoq.RemindDate
	FROM dbo.EOQY AS eoq WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON eoq.EOQYId = setting.EOQYId
	WHERE setting.NominatorId = @NominatorId
		AND eoq.IsQuarter = 0
		AND eoq.StartDate <= CAST(GETDATE() AS DATE)
	ORDER BY eoq.EndDate DESC
END