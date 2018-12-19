CREATE PROCEDURE [dbo].[NominationReport_GetPeriods]
	@IsQuarter BIT = 1
AS
/*
Created: 20181219@elton.ton
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
	WHERE IsQuarter = @IsQuarter
	ORDER BY StartDateForNominator DESC
END
