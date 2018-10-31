
CREATE PROCEDURE [dbo].[Menu_GetByEmployeeId] @EmployeeId INT
AS
/*
Created: 20180621@Tim.Trinh
Task: Get menu by EmployeeId.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT menu.MenuId
		,menu.[Name]
		,menu.[Description]
		,menu.Link
		,menu.ParentId
		,menu.DisplayOrder
	FROM dbo.Menu AS menu WITH (NOLOCK)
	WHERE menu.ParentId IS NULL
		AND menu.IsActive = 1
		AND menu.[Name] = 'Dashboard'
	
	UNION
	SELECT menu.MenuId
		,menu.[Name]
		,menu.[Description]
		,menu.Link
		,menu.ParentId
		,menu.DisplayOrder
	FROM dbo.Menu AS menu WITH (NOLOCK)
		INNER JOIN (
		SELECT DISTINCT func.FunctionId
		FROM dbo.[Function] AS func WITH (NOLOCK)
			INNER JOIN dbo.RoleFunction AS rfunc WITH (NOLOCK) ON func.FunctionId = rfunc.FunctionId
			INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON rfunc.RoleId = erole.RoleId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON erole.EmployeeId = emp.EmployeeId
		WHERE emp.EmployeeId = @EmployeeId
			AND func.IsActive = 1
			AND rfunc.Policy > 0
		) AS func ON menu.FunctionId = func.FunctionId
	WHERE menu.IsActive = 1
	ORDER BY menu.DisplayOrder
END
