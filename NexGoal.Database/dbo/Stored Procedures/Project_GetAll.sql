
CREATE PROCEDURE [dbo].[Project_GetAll]
AS
/*
Created: 20180823@Tim.Trinh
Task: Get project from redmine
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT ProjectId
		,[Name]
	FROM dbo.Project WITH (NOLOCK)
END