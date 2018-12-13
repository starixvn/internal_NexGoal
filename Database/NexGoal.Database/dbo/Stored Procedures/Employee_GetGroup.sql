
CREATE PROCEDURE [dbo].[Employee_GetGroup] @ActorId INT
AS
/*
Created: 20181102@Tim.Trinh
Task: Get group by actorId
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

	SELECT DISTINCT gr.GroupId
		,gr.[Name]
		,gr.Code
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.func_GetMember(@ActorId) AS member ON emp.EmployeeId = member.EmployeeId
		AND member.ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	ORDER BY gr.[Name]
END