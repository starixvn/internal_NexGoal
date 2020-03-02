CREATE PROCEDURE [dbo].[Period_GetCurrentPerformancePeriod]
AS
/*
Created: 20181101@Tim.Trinh
Task: get current performance period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT TOP 1 PeriodId
		,[Name]
		,StartDate
		,EndDate
		,FocusStartDate
		,FocusEndDate
		,DisciplineStartDate
		,DisciplineEndDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND DisciplineStartDate <= @Today
	ORDER BY DisciplineStartDate DESC
END
