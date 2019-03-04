
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

	DECLARE @FunctionKey VARCHAR(50) = 'PAFocus'
		,@SARoleId TINYINT = 1
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorRoleId TINYINT = 4

	CREATE TABLE #tblMembers (EmployeeId INT)

	INSERT INTO #tblMembers (EmployeeId)
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@ActorId)
	WHERE RoleId = @EvaluatorRoleId

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId IN (@HRRoleId, @ManagementRoleId)
				INNER JOIN dbo.RoleFunction AS r WITH (NOLOCK) ON r.RoleId = er.RoleId
				AND r.Policy > 0
				INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON f.FunctionId = r.FunctionId
				AND f.FunctionKey = @FunctionKey
			WHERE emp.EmployeeId = @ActorId
			
			UNION
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId = @SARoleId
			WHERE emp.EmployeeId = @ActorId
			)
	BEGIN
		INSERT INTO #tblMembers (EmployeeId)
		SELECT EmployeeId
		FROM dbo.Employee AS emp WITH (NOLOCK)
		WHERE [Enabled] = 1
			AND IsDeleted = 0
			AND NOT EXISTS (
				SELECT 1
				FROM #tblMembers AS member
				WHERE emp.EmployeeId = member.EmployeeId
				)
	END

	SELECT DISTINCT gr.GroupId
		,gr.[Name]
		,gr.Code
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblMembers AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	ORDER BY gr.[Name]

	DROP TABLE #tblMembers
END
