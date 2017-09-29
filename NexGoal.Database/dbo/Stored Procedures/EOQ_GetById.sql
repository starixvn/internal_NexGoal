CREATE PROCEDURE [dbo].[EOQ_GetById] @EOQYId INT
AS
/*
Created: 20160907@Tim.Trinh
Task: Get EOQ by Id.
DB: NexGoal
Original: 

Revisions:
    20170929@Tim.Trinh: get EndDateForCommittee
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATE = GETDATE()

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,EndDateForCommittee
		,[Description]
		,(CASE 
			WHEN StartDate > @Date THEN 0
			WHEN @Date BETWEEN StartDate
					AND EndDate THEN 1
			WHEN EndDate < @Date THEN 2 END
			) AS StatusId -- 0: not started, 1: in progress, 2:closed
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 1
		AND EOQYId = @EOQYId
END