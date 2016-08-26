
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 5/16/2016 2:20:07 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 5/16/2016 2:15:29 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 5/16/2016 1:59:43 PM ******/
/****** Object:  Stored Procedure dbo.EpaEmployee_GetByDomainUsername    Script Date: 4/6/2016 11:27:44 AM ******/

CREATE PROCEDURE [dbo].[EpaEmployee_GetByDomainUsername] --'sms\thuthi'
	@DomainUsername NVARCHAR(50) = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get user by domain user name.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeId INT

	SET @EmployeeId = (
			SELECT EmployeeId AS EmployeeId
			FROM dbo.Employee
			WHERE DomainUsername = @DomainUsername
				AND [Enabled] = 1
				AND IsDeleted = 0
			)

	SELECT DISTINCT @EmployeeId
	
	SELECT f.FunctionId
		,f.FunctionKey
		,CAST(MAX(IsRead) AS BIT) AS IsRead
		,CAST(MAX(IsHost) AS BIT) AS IsHost
	FROM (
		SELECT RoleId
			,FunctionId
			,CAST(IsRead AS INT) AS IsRead
			,CAST(IsHost AS INT) AS IsHost
		FROM dbo.RoleFunction WITH (NOLOCK)
		) AS rf
		INNER JOIN dbo.FunctionKey AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
		INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON rf.RoleId = er.RoleId
	WHERE er.EmployeeId = @EmployeeId
	GROUP BY f.FunctionId
		,FunctionKey

END
