
CREATE PROCEDURE [dbo].[NominationPeriod_Get] @IsCompleted BIT = NULL
AS
/*
Created: 20181122@Tim.Trinh
Task: Get nomination period.
DB: bodb_NexGoal
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT EOQYId AS NominationPeriodId
		,[Quarter]
		,[Year]
		,StartDateForNominator
		,EndDateForNominator
		,RemindDateForNominator
		,EndDateForCommittee
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE @IsCompleted IS NULL
		OR EndDateForCommittee < @Today
	ORDER BY StartDateForNominator DESC
END