
CREATE PROCEDURE [dbo].[Period_GetAllByEmployee] @EmployeeId INT
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
	DECLARE @EmployeeProcess INT;

	SET @EmployeeProcess = 2;

	SELECT DISTINCT p.PeriodId
		,p.NAME
		,p.StartDate
		,p.EndDate
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON pc.EPAId = epa.EPAId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = epa.PeriodId
	WHERE epa.EmployeeId = @EmployeeId
		AND pc.ProcessId = @EmployeeProcess
		AND p.IsActive = 1
		AND pc.StartDate <= GETDATE()
		AND epa.[Status] <> 2
	ORDER BY p.EndDate DESC;
END;