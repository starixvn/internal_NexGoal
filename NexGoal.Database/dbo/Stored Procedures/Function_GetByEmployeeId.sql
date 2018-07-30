CREATE PROCEDURE [dbo].[Function_GetByEmployeeId] @EmployeeId INT
AS
/*
Created: 20180621@Tim.Trinh
Task: Get all function by EmployeeId
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT func.FunctionId
		,func.FunctionKey
		,MAX(rfunc.Policy) AS Policy
	FROM dbo.RoleFunction AS rfunc WITH (NOLOCK)
		INNER JOIN dbo.[Function] AS func WITH (NOLOCK) ON rfunc.FunctionId = func.FunctionId
		INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON rfunc.RoleId = erole.RoleId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON erole.EmployeeId = emp.EmployeeId
	WHERE emp.EmployeeId = @EmployeeId
		AND func.IsActive = 1
	GROUP BY func.FunctionId
		,func.FunctionKey
END