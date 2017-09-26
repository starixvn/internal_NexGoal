



CREATE PROCEDURE [dbo].[EOY_GetQuestion]
AS
/*
Created: 20160912@Tim.Trinh
Task: Get EOY question.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT QuestionId
		,[Name]
	FROM dbo.EOYQuestion WITH (NOLOCK)
	WHERE IsActive = 1
END