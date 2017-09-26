



CREATE PROCEDURE [dbo].[EOQ_GetQuestion]
AS
/*
Created: 20160907@Tim.Trinh
Task: Get EOQ question.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT QuestionId
		,[Name]
	FROM dbo.EOQQuestion WITH (NOLOCK)
	WHERE IsActive = 1
END