


CREATE PROCEDURE [dbo].[Violation_GetById] @ViolationId INT
AS
/*
Created: 20160901@Razor.Nguyen
Task: Violation management.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT v.ViolationId
		,v.[Description]
		,v.IsActive
		,v.EffectedDate
		,g.NAME AS GroupName
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,c.EmployeeId AS CreatedBy
		,c.NickName
		,c.FullName
	FROM dbo.Violation AS v WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = v.EmployeeId
		INNER JOIN dbo.Employee AS c WITH (NOLOCK) ON c.EmployeeId = v.CreatedBy
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
	WHERE ViolationId = @ViolationId
END