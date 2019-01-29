CREATE PROCEDURE [dbo].[Report_GetPeriods]
AS
/*
Created: 20190115@Elton.ton
Task: get periods
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT PeriodId
		,[Name]
	FROM dbo.Period WITH (NOLOCK)
	WHERE EndDate < @Today
	ORDER BY EndDate DESC
END
GO