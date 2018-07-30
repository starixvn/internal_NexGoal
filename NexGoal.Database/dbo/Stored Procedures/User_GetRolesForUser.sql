
CREATE PROCEDURE [dbo].[User_GetRolesForUser] @EmployeeId INT
AS
/*
Created: 20180704@Elton.ton
Task: Get roles for user
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	SELECT rr.ResourceId AS RoleId
		,rr.Value AS [Role]
		,CAST(ISNULL(er.RoleId, 0) AS BIT) AS HasRole
	FROM dbo.RefResource AS rr WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON rr.ResourceId = er.RoleId
		AND rr.RefName = 'SystemRole'
		AND er.EmployeeId = @EmployeeId
END