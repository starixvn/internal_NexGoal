
CREATE PROCEDURE [dbo].[PAFocus_GetPeriod]
AS
/*
Created: 20181009@Tim.Trinh
Task: PA Focus
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
		,StartDate
		,EndDate
		,FocusStartDate
		,FocusEndDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND FocusStartDate <= @Today
END
