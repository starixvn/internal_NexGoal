
CREATE PROCEDURE [dbo].[Employee_GetMemberNominationResult] @ActorId INT
	,@EmployeeId INT = NULL
AS
/*
Created: 20181002@Tim.Trinh
Task: Get member nomination result.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	CREATE TABLE #tblMember (EmployeeId INT)

	INSERT INTO #tblMember (EmployeeId)
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@ActorId)
	WHERE ISNULL(@EmployeeId, EmployeeId) = EmployeeId
		AND ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)

	SELECT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,eoqy.EOQYId AS NominationPeriodId
		,nomination.StatusId
		,eoqy.EndDateForCommittee
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
		INNER JOIN dbo.EOQY AS eoqy WITH (NOLOCK) ON nomination.EOQYId = eoqy.EOQYId
		AND eoqy.EndDateForCommittee < @Today
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	ORDER BY eoqy.EndDateForCommittee DESC
		,emp.NickName

	SELECT eoqy.EOQYId AS NominationPeriodId
		,eoqy.[Quarter]
		,eoqy.[Year]
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON eoqy.EOQYId = nomination.EOQYId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON nomination.NomineeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
	WHERE eoqy.EndDateForCommittee < @Today

	DROP TABLE #tblMember
END
