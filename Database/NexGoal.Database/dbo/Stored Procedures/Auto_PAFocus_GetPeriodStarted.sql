
CREATE PROCEDURE [dbo].[Auto_PAFocus_GetPeriodStarted]
AS
/*
Created: 20181221@Tim.Trinh
Task: get period started.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT PeriodId
	FROM dbo.Period WITH (NOLOCK)
	WHERE FocusStartDate = @Today
		AND IsActive = 1
END