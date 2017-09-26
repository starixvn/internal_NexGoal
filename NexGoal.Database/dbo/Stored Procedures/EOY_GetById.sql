


CREATE PROCEDURE [dbo].[EOY_GetById] @EOQYId INT
AS
/*
Created: 20160907@Tim.Trinh
Task: Get EOY by Id.
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
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,[Description]
		,(CASE 
			WHEN StartDate > @Date THEN 0
			WHEN @Date BETWEEN StartDate
					AND EndDate THEN 1
			WHEN EndDate < @Date THEN 2 END
			) AS StatusId -- 0: not started, 1: in progress, 2:closed
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 0
		AND EOQYId = @EOQYId
END