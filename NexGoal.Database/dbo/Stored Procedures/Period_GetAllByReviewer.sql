

CREATE PROCEDURE [dbo].[Period_GetAllByReviewer] @ReviewerId INT
AS
/*
Created: 220160818@razor.nguyen
Task: Access page
DB: NexGoal
Original:

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @ReviewerProcess INT;

	SET @ReviewerProcess = 4;

	SELECT DISTINCT p.PeriodId
		,p.NAME
		,p.StartDate
		,p.EndDate		
		,tg.TargetTypeId
		,tg.Name
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = epa.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON pc.EPAId = epa.EPAId
		INNER JOIN dbo.TargetType as tg with(Nolock) on tg.TargetTypeId = p.TargetTypeId
	WHERE pc.[EvaluatorId] = @ReviewerId
		AND pc.ProcessId = @ReviewerProcess
		AND p.IsActive = 1
		AND pc.StartDate <= GETDATE()
		AND epa.[Status] <> 2
	ORDER BY p.EndDate DESC;
END;