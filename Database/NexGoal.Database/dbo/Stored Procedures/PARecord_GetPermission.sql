
CREATE PROCEDURE [dbo].[PARecord_GetPermission] @PARecordId INT
	,@ActorId INT
AS
/*
Created: 20190121@Tim.Trinh
Task: get permission of user.
DB: bodb_NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Permission TINYINT = 0 -- don't permission
		,@SARoleId TINYINT = 1
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@EmployeeId INT

	SELECT @EmployeeId = EmployeeId
	FROM dbo.PARecord WITH (NOLOCK)
	WHERE PARecordId = @PARecordId

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId IN (@SARoleId, @ManagementRoleId)
			WHERE emp.EmployeeId = @ActorId
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@ActorId)
			WHERE EmployeeId = @EmployeeId
				AND ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			)
		OR @ActorId = @EmployeeId
	BEGIN
		SET @Permission = 1
	END

	SELECT @Permission
END