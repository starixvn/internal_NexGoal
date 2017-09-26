
CREATE PROCEDURE [dbo].[EpaEmployee_GetByDomainUsername] 
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

	IF (@EmployeeId IS NOT NULL AND
			NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS er WITH (NOLOCK)
				WHERE er.EmployeeId = @EmployeeId
					AND er.RoleId = 6
				)
			)
	BEGIN
		INSERT INTO dbo.EmployeeRole
		VALUES (
			@EmployeeId
			,6
			,NULL
			,NULL
			)
	END

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
