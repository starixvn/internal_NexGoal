
CREATE PROCEDURE [dbo].[Employee_GetById] @EmployeeId INT
AS
/*
Created: 20180704@Elton.ton
Task: Get employee by id
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
		,e.Code
		,e.FullName
		,e.NickName
		,jt.[Name] AS JobTitle
		,g.Code AS GroupCode
		,g.[Name] AS GroupName
		,STUFF((
				SELECT ', ' + rr.Value
				FROM dbo.EmployeeRole AS er WITH (NOLOCK)
					INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON er.RoleId = rr.ResourceId
					AND rr.RefName = 'SystemRole'
					AND er.EmployeeId = e.EmployeeId
				FOR XML PATH('')
				), 1, 2, '') AS Roles
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE e.EmployeeId = @EmployeeId
END
