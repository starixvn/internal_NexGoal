
CREATE PROCEDURE [dbo].[Auto_Period_GetPeriodStarted]
AS
/*
Created: 20180919@Tim.Trinh
Task: get period started.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @today DATE = DATEADD(DAY, -1, GETDATE())

	SELECT PeriodId
	FROM dbo.Period WITH (NOLOCK)
	WHERE StartDate = @today
		AND IsActive = 1
END
