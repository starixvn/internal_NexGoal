
CREATE PROCEDURE [dbo].[PARecord_GetPeriodByActor] @ActorId INT
AS
/*
Created: 20180920@Tim.Trinh
Task: Get period by actor.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@HRProcessId TINYINT = 1

	SELECT DISTINCT period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId > @HRProcessId
		AND ISNULL(process.StartDate, @Today) <= @Today
	ORDER BY period.EndDate DESC
END
