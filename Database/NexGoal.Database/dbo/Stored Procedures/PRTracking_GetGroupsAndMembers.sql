CREATE PROCEDURE [dbo].[PRTracking_GetGroupsAndMembers] 
	@ActorId INT
AS
/*
Created: 20190506@Elton.Ton
Task: Get groups and members.
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE	@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@PSTRoleId TINYINT = 7


	CREATE TABLE #member(EmployeeId INT)

	IF EXISTS (
		SELECT 1
		FROM dbo.EmployeeRole AS er WITH (NOLOCK)
		WHERE er.EmployeeId = @ActorId 
			AND er.RoleId IN (@HRRoleId, @ManagementRoleId, @PSTRoleId)
	)
	BEGIN
		INSERT INTO #member(EmployeeId)
		SELECT e.EmployeeId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE e.IsDeleted = 0
			AND e.[Enabled] = 1
	END
	ELSE
	BEGIN
		INSERT INTO #member(EmployeeId)
		SELECT EmployeeId
		FROM dbo.func_GetMember(@ActorId)
	END

	INSERT INTO #member(EmployeeId) VALUES (@ActorId)

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
END
