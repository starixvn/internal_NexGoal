
CREATE PROCEDURE [dbo].[Period_GetAll]
AS
/*
Created: 20181016@Elton.Ton
Task: get  all period
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT p.PeriodId
		,p.[Name]
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE p.IsActive = 1
	ORDER BY p.StartDate DESC
END
