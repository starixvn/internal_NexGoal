CREATE PROCEDURE [dbo].[Employee_GetMemberByProcessId] @ActorId INT
	,@processId SMALLINT
AS
/*
Created: 20181114@Oliver.Dang
Task: Get member by process Id.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	-- process
	SELECT DISTINCT emp.EmployeeId
		,emp.FullName
		,emp.NickName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.func_GetMember(@ActorId) AS member ON emp.EmployeeId = member.EmployeeId
		AND member.ProcessId = @processId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	ORDER BY emp.NickName
END
