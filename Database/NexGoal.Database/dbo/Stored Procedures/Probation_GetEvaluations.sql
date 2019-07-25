CREATE PROCEDURE [dbo].[Probation_GetEvaluations]
	@ActorId INT
	,@EmployeeId INT = NULL
AS
/*
Created: 20190520@Elton.ton
Task: get probationer's evaluation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
	DECLARE @ReviewerProcessId TINYINT = 5
	 
	SELECT pe.EmployeeId
		,pe.IsQualified
		,e.NickName
		,e.FullName
		,e.Code
		,e.GroupId
		,g.GroupId AS GroupName
		,pe.SelfScore
		,pe.SelfComment
		,pe.EvaluatorScore
		,pe.EvaluatorComment
		,pe.ReviewerComment
		,pe.IsQualifiedByEvaluator
	FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON pe.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE EXISTS (
		SELECT 1
		FROM dbo.ProbationSetting ps WITH (NOLOCK)
		WHERE ps.EmployeeId = pe.EmployeeId AND ps.EvaluatorId = @ActorId
	)
		AND (@EmployeeId IS NULL OR pe.EmployeeId = @EmployeeId)
		AND EXISTS (
			SELECT 1
			FROM dbo.ProbationSetting ps WITH (NOLOCK)
			WHERE ps.EmployeeId = pe.EmployeeId	AND ps.ProcessId = @ReviewerProcessId
				AND @Today <= ps.ToDate
		)

	SELECT ps.EmployeeId
		,ps.EvaluatorId
		,ps.ProcessId
		,rr.Value AS Process
		,ps.FromDate
		,ps.ToDate
		,e.NickName AS EvaluatorNickname
		,e.FullName AS EvaluatorFullName
	FROM dbo.ProbationSetting ps WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON ps.EvaluatorId = e.EmployeeId
		INNER JOIN dbo.RefResource rr WITH (NOLOCK) ON rr.ResourceId = ps.ProcessId AND rr.RefName = 'PerformanceProcess'
	WHERE EXISTS (
		SELECT 1
		FROM dbo.ProbationSetting ps1 WITH (NOLOCK)
		WHERE ps.EmployeeId = ps1.EmployeeId AND ps1.EvaluatorId = @ActorId
			AND (@EmployeeId IS NULL OR ps1.EmployeeId = @EmployeeId)
	)
		AND EXISTS (
			SELECT 1
			FROM dbo.ProbationSetting ps1 WITH (NOLOCK)
			WHERE ps.EmployeeId = ps1.EmployeeId AND ps1.ProcessId = @ReviewerProcessId
				AND @Today <= ps1.ToDate
		)
		
END