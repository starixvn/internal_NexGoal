
CREATE PROCEDURE [dbo].[Employee_GetGroups]
AS
/*
Created: 20180704@Elton.ton
Task: Get groups
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	SELECT g.GroupId
		,g.Code
		,g.[Name]
	FROM dbo.[Group] AS g WITH (NOLOCK)
	WHERE g.IsDeleted = 0
END