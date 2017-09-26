


CREATE PROCEDURE [dbo].[PAFocus_GetProcessByPeriodId] @PeriodId INT
AS
/*
Created: 20160830@Razor.Nguyen
Task: PA Focus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT PeriodId
		,ProcessId
		,StartDate
		,EndDate
	FROM dbo.PAFocusProcess AS pc WITH (NOLOCK)
		INNER JOIN dbo.PAFocus AS f WITH (NOLOCK) ON f.PAFocusId = pc.PAFocusId
	WHERE f.PeriodId = @PeriodId
	ORDER BY ProcessId
END