
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

	DECLARE @EmployeeRoleId SMALLINT

	SELECT @EmployeeRoleId = ResourceId
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'SystemRole'
		AND Value = 'Employee'

	SELECT menu.MenuId
		,menu.[Name]
		,menu.[Description]
		,menu.Link
		,menu.ParentId
		,menu.DisplayOrder
	FROM dbo.Menu AS menu WITH (NOLOCK)
	WHERE menu.MenuId = 1
		AND menu.ParentId IS NULL
		AND menu.IsActive = 1
	
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
			LEFT JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON rfunc.RoleId = erole.RoleId
		WHERE (
				rfunc.RoleId = @EmployeeRoleId
				OR erole.EmployeeId = @EmployeeId
				)
			AND func.IsActive = 1
			AND rfunc.Policy > 0
		) AS func ON menu.FunctionId = func.FunctionId
	WHERE menu.IsActive = 1
	ORDER BY menu.DisplayOrder
END
