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

	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,EndDateForCommittee
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 1
		AND EOQYId = @EOQYId
END