

CREATE PROCEDURE [dbo].[Period_GetToGeneratePAFocus]
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

	SELECT p.PeriodId
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE p.EndDate < GETDATE()
		AND p.PeriodId NOT IN (
			SELECT DISTINCT PeriodId
			FROM dbo.PAFocus
			)
END