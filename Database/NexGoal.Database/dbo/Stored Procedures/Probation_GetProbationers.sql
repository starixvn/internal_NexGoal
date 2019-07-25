CREATE PROCEDURE [dbo].[Probation_GetProbationers]

AS
/*
Created: 20190520@Elton.ton
Task: get probationer settings
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;
	 
	SELECT pe.EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
		,g.GroupId AS GroupName
	FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON pe.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE e.ConfirmedDate IS NULL AND e.Enabled = 1 AND e.IsDeleted = 0

	SELECT ps.EmployeeId
		,ps.EvaluatorId
		,ps.ProcessId
		,rr.Value AS Process
		,ps.FromDate
		,ps.ToDate
		,evaluator.NickName AS EvaluatorNickName
		,evaluator.FullName AS EvaluatorFullName
	FROM dbo.ProbationSetting ps WITH (NOLOCK)
		INNER JOIN dbo.Employee evaluator WITH (NOLOCK) ON ps.EvaluatorId = evaluator.EmployeeId
		INNER JOIN dbo.RefResource rr WITH (NOLOCK) ON rr.ResourceId = ps.ProcessId AND rr.RefName = 'PerformanceProcess'
	WHERE EXISTS (
		SELECT 1
		FROM dbo.Employee e WITH (NOLOCK)
		WHERE ps.EmployeeId = e.EmployeeId
			AND e.ConfirmedDate IS NULL AND e.Enabled = 1 AND e.IsDeleted = 0
	)		
END