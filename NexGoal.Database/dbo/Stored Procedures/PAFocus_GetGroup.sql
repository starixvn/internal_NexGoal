
CREATE PROCEDURE [dbo].[PAFocus_GetGroup] @ActorId INT
AS
/*
Created: 20181008@Tim.Trinh
Task: PA Focus
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SARoleId TINYINT = 1
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorRoleId TINYINT = 4
		,@ReviewerRoleId TINYINT = 5

	CREATE TABLE #tblMembers (EmployeeId INT)

	INSERT INTO #tblMembers (EmployeeId)
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@ActorId)
	WHERE RoleId IN (@SARoleId, @ManagementRoleId, @EvaluatorRoleId)

	SELECT DISTINCT gr.GroupId
		,gr.[Name]
		,gr.Code
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblMembers AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId

	DROP TABLE #tblMembers
END
