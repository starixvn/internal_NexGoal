
CREATE PROCEDURE [dbo].[Employee_GetRolesById] @EmployeeId INT
AS
/*
Created: 20180704@Elton.ton
Task: Get roles by employee
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeRoleId SMALLINT

	SELECT @EmployeeRoleId = ResourceId
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'SystemRole'
		AND Value = 'Employee'

	SELECT rr.ResourceId AS RoleId
		,rr.Value AS [Role]
		,CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole AS er WITH (NOLOCK)
					WHERE er.RoleId = rr.ResourceId
						AND er.EmployeeId = @EmployeeId
					) THEN 1
			WHEN rr.ResourceId = @EmployeeRoleId THEN 1
			ELSE 0
			END AS HasRole
	FROM dbo.RefResource AS rr WITH (NOLOCK)
	WHERE rr.RefName = 'SystemRole'
	ORDER BY ResourceId
END
