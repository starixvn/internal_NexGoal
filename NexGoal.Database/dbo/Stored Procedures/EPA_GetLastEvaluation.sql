
CREATE PROCEDURE [dbo].[EPA_GetLastEvaluation] @EmployeeId INT
	,@TargetTypeId INT
AS
/*
Created: 20160808@Razor.Nguyen
Task: Last Rating
DB: NexGoal
Original: 
 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds INTId
		,@EpaStatusIsClosed TINYINT
		,@EvaluatorProcess INT
		,@Processes ProcessTable;

	SET @EpaStatusIsClosed = 1;
	SET @EvaluatorProcess = 3;

	INSERT INTO @EPAIds
	SELECT TOP 1 epa.EPAId
	FROM dbo.EPA epa WITH (NOLOCK)
		INNER JOIN dbo.Period p WITH (NOLOCK) ON p.PeriodId = epa.PeriodId
	WHERE epa.EmployeeId = @EmployeeId
		AND epa.[Status] = @EpaStatusIsClosed
		AND p.TargetTypeId = @TargetTypeId
		AND p.IsActive = 1
	ORDER BY p.EndDate DESC;

	INSERT INTO @Processes (
		ProcessId
		,EvaluatorId
		)
	SELECT DISTINCT @EvaluatorProcess
		,pc.EvaluatorId
	FROM @EPAIds AS epa
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.Id = pc.EPAId
	WHERE pc.ProcessId = @EvaluatorProcess

	-- KPI Group
	EXECUTE dbo.EPA_GetKPIGroup @EPAIds

	EXECUTE dbo.[EPA_GetKPIGroupScore] @EPAIds
		,@EvaluatorProcess;

	EXECUTE dbo.EPA_GetKPI @EPAIds

	-- Evaluation per KPI
	EXECUTE EPA_GetEvaluation @EPAIds
		,@Processes
END;