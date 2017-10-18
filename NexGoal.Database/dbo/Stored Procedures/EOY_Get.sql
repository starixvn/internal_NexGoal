CREATE PROCEDURE [dbo].[EOY_Get] @Year INT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20160912@Tim.Trinh
Task: Get EOY.
DB: NexGoal
Original: 

Revisions:
    20170929@Tim.Trinh: get EndDateForCommittee  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATE = GETDATE()

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,EndDateForCommittee
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 0
		AND ISNULL(@Year, [Year]) = [Year]
		AND (
			@StatusId IS NULL
			OR (
				@StatusId = 0 -- not started
				AND StartDate > @Date
				)
			OR (
				@StatusId = 1 -- in progress
				AND @Date BETWEEN StartDate
					AND EndDateForCommittee
				)
			OR (
				@StatusId = 2 -- closed
				AND EndDateForCommittee < @Date
				)
			)
	ORDER BY [Year] DESC
END