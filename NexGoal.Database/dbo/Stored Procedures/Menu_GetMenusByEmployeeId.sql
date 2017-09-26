

CREATE PROCEDURE [dbo].[Menu_GetMenusByEmployeeId] @EmployeeId INT
AS
/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT me.MenuId
		,me.[Name]
		,me.[Text]
		,me.[Link]
		,me.ParentId
		,me.FunctionId
		,me.DisplayOrder
		,me.IsActive
	FROM dbo.EmployeeRole AS empRo WITH (NOLOCK)
		INNER JOIN dbo.RoleFunction AS roFunc WITH (NOLOCK) ON empRo.RoleId = roFunc.RoleId
		AND roFunc.IsRead = 1
		INNER JOIN dbo.Menu AS me WITH (NOLOCK) ON roFunc.FunctionId = me.FunctionId
	WHERE empRo.EmployeeId = @EmployeeId
		AND me.IsActive = 1
	
	UNION ALL
	SELECT DISTINCT me.MenuId
		,me.[Name]
		,me.[Text]
		,me.Link
		,me.ParentId
		,me.FunctionId
		,me.DisplayOrder
		,me.IsActive
	FROM dbo.Menu AS me WITH (NOLOCK)
	WHERE FunctionId IS NULL
		AND IsActive = 1
		AND (me.MenuId IN (
			SELECT DISTINCT me.ParentId
			FROM dbo.EmployeeRole AS empRo WITH (NOLOCK)
				INNER JOIN dbo.RoleFunction AS roFunc WITH (NOLOCK) ON empRo.RoleId = roFunc.RoleId
				AND roFunc.IsRead = 1
				INNER JOIN dbo.Menu AS me WITH (NOLOCK) ON roFunc.FunctionId = me.FunctionId
			WHERE empRo.EmployeeId = @EmployeeId
				AND me.IsActive = 1
			) OR me.Name = 'MyDashboard')
END;
