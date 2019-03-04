
CREATE PROCEDURE [dbo].[PAFocus_GetMember] @ActorId INT
	,@GroupIds VARCHAR(MAX) = NULL
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

	CREATE TABLE #tblGroupIds (GroupId INT)

	CREATE TABLE #tblMembers (EmployeeId INT)

	INSERT INTO #tblGroupIds (GroupId)
	SELECT DISTINCT val
	FROM dbo.func_SplitString(@GroupIds, ',')

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

	SELECT emp.EmployeeId
		,emp.FullName
		,emp.NickName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblMembers AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN #tblGroupIds AS tblgr ON gr.GroupId = tblgr.GroupId
	ORDER BY emp.NickName

	DROP TABLE #tblGroupIds

	DROP TABLE #tblMembers
END
