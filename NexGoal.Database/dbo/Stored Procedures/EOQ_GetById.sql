
CREATE PROCEDURE [dbo].[EOQ_GetById] @EOQId INT
AS
/*
Created: 20180717@Tim.Trinh
Task: Get EOQ by Id.
DB: bodb_NexGoal
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDateForNominator
		,EndDateForNominator
		,RemindDateForNominator
		,EndDateForCommittee
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 1
		AND EOQYId = @EOQId
END
