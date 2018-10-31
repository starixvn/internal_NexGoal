
CREATE PROCEDURE [dbo].[PerformanceRecord_GetMember_notused] @ActorId INT
AS
/*
Created: 20180913@Tim.Trinh
Task: Get member.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT DISTINCT emp.EmployeeId
		,emp.FullName
		,emp.NickName
		,emp.Code
	FROM dbo.func_GetMember(@ActorId) AS member
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON member.EmployeeId = emp.EmployeeId
	WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId)
	ORDER BY emp.NickName
END
