
CREATE PROCEDURE [dbo].[EOY_GetById] @EOYId INT
AS
/*
Created: 20180718@Tim.Trinh
Task: Get EOY by Id.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT EOQYId
		,[Year]
		,StartDateForNominator
		,EndDateForNominator
		,RemindDateForNominator
		,EndDateForCommittee
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 0
		AND EOQYId = @EOYId
END