CREATE PROCEDURE [dbo].[ProbationReport_Get]
	@FromDate DATE = NULL
	,@ToDate DATE = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@IsQualifiedByEvaluator BIT = NULL
	,@IsQualified BIT = NULL
	,@ActorId INT
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
	
	DECLARE	@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@SelfProcessId TINYINT = 2
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@HRProcessId TINYINT = 1

	CREATE TABLE #member(EmployeeId INT, ProcessId TINYINT)

	 SELECT e.val AS EmployeeId
	 INTO #selectedEmployee
	 FROM dbo.func_SplitString(@EmployeeIds,',') AS e

	IF EXISTS (
		SELECT 1
		FROM dbo.EmployeeRole AS er WITH (NOLOCK)
		WHERE er.EmployeeId = @ActorId 
			AND er.RoleId IN (@HRRoleId, @ManagementRoleId)
	)
	BEGIN
		INSERT INTO #member(EmployeeId, ProcessId)
		SELECT EmployeeId
			,@HRProcessId
		FROM #selectedEmployee
	END
	ELSE
	BEGIN
		INSERT INTO #member (EmployeeId,ProcessId)
		SELECT member.EmployeeId, MIN(member.ProcessId) AS ProcessId
		FROM dbo.func_GetMember(@ActorId) AS member
			INNER JOIN #selectedEmployee se ON member.EmployeeId = se.EmployeeId
		WHERE member.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			AND EXISTS (
					SELECT 1
					FROM dbo.ProbationEvaluation AS pe WITH (NOLOCK)
					WHERE member.EmployeeId = pe.EmployeeId
			)
		GROUP BY member.EmployeeId
		UNION 
		SELECT ps.EmployeeId, ps.ProcessId
		FROM dbo.ProbationSetting ps WITH (NOLOCK)
			INNER JOIN #selectedEmployee se ON ps.EmployeeId = se.EmployeeId
		WHERE ps.EmployeeId = @ActorId AND ps.EvaluatorId = @ActorId AND ps.ProcessId = @SelfProcessId
	END

	SELECT pe.EmployeeId
		,pe.IsQualified
		,e.NickName
		,e.FullName
		,e.Code
		,e.GroupId
		,e.JoinedDate
		,jt.Name AS JobTitle
		,g.Name AS GroupName
		,pe.SelfScore
		,pe.SelfComment
		,pe.EvaluatorScore
		,pe.EvaluatorComment
		,pe.ReviewerComment
		,(CASE 
				WHEN m.ProcessId = @EvaluatorProcessId OR m.ProcessId = @ReviewerProcessId  OR m.ProcessId = @HRProcessId THEN pe.IsQualifiedByEvaluator
				ELSE NULL
			END) AS IsQualifiedByEvaluator
		,CAST((CASE 
				WHEN m.ProcessId = @EvaluatorProcessId OR m.ProcessId = @ReviewerProcessId OR m.ProcessId = @HRProcessId THEN 1
				ELSE 0
		END) AS BIT) AS CanViewEvaluatorResult
	FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON pe.EmployeeId = e.EmployeeId
		INNER JOIN dbo.JobTitle jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN #member m ON pe.EmployeeId = m.EmployeeId
	WHERE ((@FromDate IS NULL AND @ToDate IS NULL) OR e.JoinedDate BETWEEN @FromDate AND @ToDate)
		AND (@IsQualifiedByEvaluator IS NULL OR pe.IsQualifiedByEvaluator = @IsQualifiedByEvaluator)
		AND (@IsQualified IS NULL OR pe.IsQualified = @IsQualified)
		AND EXISTS (
			SELECT 1
			FROM dbo.ProbationSetting ps WITH (NOLOCK)
			WHERE ps.EmployeeId = pe.EmployeeId AND ps.ProcessId = @ReviewerProcessId AND ps.ToDate < @Today
		)
	ORDER BY e.JoinedDate DESC

	DROP TABLE #selectedEmployee
	DROP TABLE #member
END