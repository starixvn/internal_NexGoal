
CREATE PROCEDURE [dbo].[EOQ_GetYear]
AS
/*
Created: 20160906@Razor.Nguyen
Task: Get EOQ.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT [Year]
	FROM dbo.EOQY WITH (NOLOCK)
	WHERE IsQuarter = 1
	ORDER BY [Year] desc
END