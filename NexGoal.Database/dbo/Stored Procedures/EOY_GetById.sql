CREATE PROCEDURE [dbo].[EOY_GetById] @EOQYId INT
AS
/*
Created: 20160907@Tim.Trinh
Task: Get EOY by Id.
DB: NexGoal
Original: 

Revisions:
    20170929@Tim.Trinh: get EndDateForCommittee 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT EOQYId
		,[Year]
		,StartDate
		,EndDate
		,RemindDate
		,EndDateForCommittee
		,[Description]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 0
		AND EOQYId = @EOQYId
END