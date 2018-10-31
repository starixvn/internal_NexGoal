
CREATE PROCEDURE [dbo].[EOQ_Get] @Year INT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20180711@Tim.Trinh
Task: Get EOQ.
DB: bodb_NexGoal
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATE = GETDATE()

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
		AND ISNULL(@Year, [Year]) = [Year]
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
