


CREATE PROCEDURE [dbo].[EOY_Get] @Year INT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20160912@Tim.Trinh
Task: Get EOY.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATE

	SET @Date = CAST(GETDATE() AS DATE)

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 0
		AND (
			@Year IS NULL
			OR [Year] = @Year
			)
		AND (
			@StatusId IS NULL
			OR (
				@StatusId = 0 -- not started
				AND StartDate > @Date
				)
			OR (
				@StatusId = 1 -- in progress
				AND @Date BETWEEN StartDate
					AND EndDate
				)
			OR (
				@StatusId = 2 -- closed
				AND EndDate < @Date
				)
			)
	ORDER BY [Year] DESC
END