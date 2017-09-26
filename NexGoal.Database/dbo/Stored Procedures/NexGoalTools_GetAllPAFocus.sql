


CREATE PROCEDURE [dbo].[NexGoalTools_GetAllPAFocus] @PAFocusIdFrom INT
	,@PAFocusIdTo INT
AS
/*
Created: 20161019@razor.nguyen
Task: update score.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT PAFocusId
		,Contribution AS EncryptedContribution
		,BonusGiven AS EncryptedBonusGiven
		,IsReadyPromotion AS EncryptedIsReadyPromotion
		,IsPoorPerformance AS EncryptedIsPoorPerformance
	FROM dbo.PAFocus WITH (NOLOCK)
	WHERE PAFocusId >= @PAFocusIdFrom
		AND PAFocusId < @PAFocusIdTo
END