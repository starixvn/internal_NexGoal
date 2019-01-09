CREATE PROCEDURE [dbo].[PMSetting_GetProjectManagers]
AS
/*
Created: 20190107@Elton.Ton
Task: get PMs
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
		,e.Code
		,e.FullName
		,e.NickName
	FROM dbo.Employee e WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole er WITH (NOLOCK)
			WHERE er.EmployeeId = e.EmployeeId AND er.RoleId = 6
		)
	ORDER BY e.NickName
END

GO


