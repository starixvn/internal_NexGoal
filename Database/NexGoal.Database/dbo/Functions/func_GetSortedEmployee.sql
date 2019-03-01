
CREATE FUNCTION [dbo].[func_GetSortedEmployee] (@ActorId INT)
RETURNS @result TABLE (
	EmployeeId INT 
	,RankIndex SMALLINT IDENTITY(1,1)
	)
AS
BEGIN
	DECLARE @IsHRRole BIT = 0
	IF (
			EXISTS (
				SELECT 1
				FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
					AND er.RoleId IN (2, 3)
				WHERE emp.EmployeeId = @ActorId
				)
			)
	BEGIN
		SET @IsHRRole = 1
	END 
	
	INSERT INTO @result(EmployeeId)
	SELECT EmployeeId
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	ORDER BY CASE WHEN @IsHRRole = 1 THEN 3
			WHEN e.EmployeeId = @ActorId THEN 0
			WHEN g.LeaderId = @ActorId
				THEN 1
			ELSE 2
			END
		,g.Rank
		,g.Name
		,e.Code

	RETURN
END