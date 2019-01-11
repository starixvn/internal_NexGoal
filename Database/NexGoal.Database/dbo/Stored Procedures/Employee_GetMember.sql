
CREATE PROCEDURE [dbo].[Employee_GetMember] @ActorId INT
	,@Keyword NVARCHAR(250) = NULL
AS
/*
Created: 20181003@Tim.Trinh
Task: Get member.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@ProbationDuration SMALLINT
	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT TOP 1 @ProbationDuration = CAST(Value AS SMALLINT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationDuration'

	SELECT DISTINCT emp.EmployeeId
		,emp.FullName
		,emp.NickName
		,(CASE 
			WHEN @Today BETWEEN emp.JoinedDate
					AND DATEADD(DAY, @ProbationDuration, emp.JoinedDate) THEN 1
			ELSE 0 END
			) AS IsProbation
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.func_GetMember(@ActorId) AS member ON emp.EmployeeId = member.EmployeeId
		AND member.ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			emp.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR emp.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			)
	ORDER BY emp.NickName
END
