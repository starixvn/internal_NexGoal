
CREATE PROCEDURE [dbo].[EOQ_GetYear]
AS
/*
Created: 20180711@Tim.Trinh
Task: Get EOQ.
DB: bodb_NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 1
			)
	BEGIN
		SELECT DISTINCT [Year]
		FROM dbo.EOQY WITH (NOLOCK)
		WHERE IsQuarter = 1
		ORDER BY [Year] DESC
	END
	ELSE
	BEGIN
		SELECT YEAR(GETDATE()) AS [Year]
	END
END