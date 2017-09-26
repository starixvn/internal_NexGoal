
CREATE PROCEDURE [dbo].[PAFocus_GetProcessId] @EvaluatorId INT
	,@PeriodId INT
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

	SELECT DISTINCT pc.ProcessId
	FROM dbo.PAFocus AS f WITH (NOLOCK)
		INNER JOIN dbo.PAFocusProcess AS pc WITH (NOLOCK) ON f.PAFocusId = pc.PAFocusId
	WHERE f.PeriodId = @PeriodId
		AND pc.EvaluatorId = @EvaluatorId
END