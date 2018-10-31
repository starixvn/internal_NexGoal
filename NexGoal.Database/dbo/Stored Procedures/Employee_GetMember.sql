
CREATE PROCEDURE [dbo].[Employee_GetMember] @ActorId INT
	,@Keyword NVARCHAR(250) = NULL
AS
/*
Created: 20181003@Tim.Trinh
Task: Get member.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT emp.EmployeeId
		,emp.FullName
		,emp.NickName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.func_GetMember(@ActorId) AS member ON emp.EmployeeId = member.EmployeeId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			emp.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR emp.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			)
	ORDER BY emp.NickName
END