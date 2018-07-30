CREATE PROCEDURE [dbo].[Employee_GetByDomainUsername] @DomainUsername NVARCHAR(50) = NULL
AS
/*
Created: 20180621@Tim.Trinh
Task: Get user by domain user name.
DB: bodb_NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT EmployeeId
		,FullName
		,NickName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE DomainUsername = @DomainUsername
		AND [Enabled] = 1
		AND IsDeleted = 0
END