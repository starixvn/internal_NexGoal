CREATE PROCEDURE [dbo].[ProbationReport_GetGroupsAndEmployees] 
	@ActorId INT
AS
/*
Created: 20190701@Elton.Ton
Task: Get groups and members.
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE	@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	CREATE TABLE #member(EmployeeId INT)

	IF EXISTS (
		SELECT 1
		FROM dbo.EmployeeRole AS er WITH (NOLOCK)
		WHERE er.EmployeeId = @ActorId 
			AND er.RoleId IN (@HRRoleId, @ManagementRoleId)
	)
	BEGIN
		INSERT INTO #member(EmployeeId)
		SELECT e.EmployeeId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE e.IsDeleted = 0
			AND e.[Enabled] = 1
			AND EXISTS (
				SELECT 1
				FROM dbo.ProbationEvaluation AS pe WITH (NOLOCK)
				WHERE e.EmployeeId = pe.EmployeeId
			)
	END
	ELSE
	BEGIN
		INSERT INTO #member (EmployeeId)
		SELECT DISTINCT member.EmployeeId
		FROM dbo.func_GetMember(@ActorId) AS member
		WHERE member.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			AND EXISTS (
					SELECT 1
					FROM dbo.ProbationEvaluation AS pe WITH (NOLOCK)
					WHERE member.EmployeeId = pe.EmployeeId
			)

		IF EXISTS (SELECT 1 FROM dbo.ProbationEvaluation pe WITH (NOLOCK) WHERE pe.EmployeeId = @ActorId)
		BEGIN
			INSERT INTO #member (EmployeeId)
			SELECT @ActorId
		END
	END

	SELECT g.GroupId
		,g.Name
	FROM dbo.[Group] g WITH (NOLOCK)
	WHERE EXISTS (
		SELECT 1
		FROM dbo.Employee e WITH (NOLOCK)
			INNER JOIN #member m ON e.EmployeeId = m.EmployeeId
		WHERE g.GroupId = e.GroupId
	)
	ORDER BY g.Name

	SELECT e.EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
	FROM dbo.Employee e WITH (NOLOCK)
	WHERE EXISTS(
		SELECT 1
		FROM #member m
		WHERE e.EmployeeId = m.EmployeeId
	)
	ORDER BY e.NickName

	DROP TABLE #member
END