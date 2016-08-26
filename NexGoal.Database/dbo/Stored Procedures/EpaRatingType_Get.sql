/****** Object:  Stored Procedure dbo.EpaRatingType_Get    Script Date: 7/21/2016 2:45:54 PM ******/

CREATE PROCEDURE [dbo].[EpaRatingType_Get] 
AS
/*
Created: 20151014@Josh.Phan
Task: Get rating type.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT CAST([Value] AS INT) AS RatingTypeId
		,[Name]
	FROM [dbo].[LookupData] WITH (NOLOCK)
	WHERE [LookupId] = 8 -- Rating Type
END
