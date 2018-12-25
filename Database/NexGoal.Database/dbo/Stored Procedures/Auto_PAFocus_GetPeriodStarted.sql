
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

	DECLARE @Tomorrow DATE = DATEADD(DAY, 1, GETDATE())

	SELECT PeriodId
	FROM dbo.Period WITH (NOLOCK)
	WHERE FocusStartDate = @Tomorrow
		AND IsActive = 1
END