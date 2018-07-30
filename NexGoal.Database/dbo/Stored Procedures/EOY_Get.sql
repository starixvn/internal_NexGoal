
CREATE PROCEDURE [dbo].[EOY_Get] @StatusId TINYINT = NULL
AS
/*
Created: 20180718@Tim.Trinh
Task: Get EOY.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATE = GETDATE()

	SELECT EOQYId
		,[Year]
		,StartDateForNominator
		,EndDateForNominator
		,RemindDateForNominator
		,EndDateForCommittee
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 0
		AND (
			@StatusId IS NULL
			OR (
				@StatusId = 0 -- not started
				AND StartDateForNominator > @Date
				)
			OR (
				@StatusId = 1 -- in progress
				AND @Date BETWEEN StartDateForNominator
					AND EndDateForCommittee
				)
			OR (
				@StatusId = 2 -- closed
				AND EndDateForCommittee < @Date
				)
			)
	ORDER BY [Year] DESC
END