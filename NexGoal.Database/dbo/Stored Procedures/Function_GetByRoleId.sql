
CREATE PROCEDURE [dbo].[Function_GetByRoleId] @RoleId SMALLINT
AS
/*
Created: 20180629@Tim.Trinh
Task: Get function by role Id.
DB: bodb_NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT func.FunctionId
		,func.FunctionKey
		,rfunc.Policy
	FROM dbo.[Function] AS func WITH (NOLOCK)
		INNER JOIN dbo.RoleFunction AS rfunc WITH (NOLOCK) ON func.FunctionId = rfunc.FunctionId
		AND rfunc.RoleId = @RoleId
	WHERE func.IsActive = 1
END