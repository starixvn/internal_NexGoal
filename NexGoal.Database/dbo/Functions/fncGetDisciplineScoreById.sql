/****** Object:  User Defined Function dbo.fncGetDisciplineScoreById    Script Date: 7/21/2016 2:45:42 PM ******/
CREATE FUNCTION [dbo].[fncGetDisciplineScoreById] (@DisciplineTypeId INT)
RETURNS DECIMAL(5, 2)
AS
/*
Created: 20160421@Tim.Trinh
Task: Get discipline score by id.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @Score DECIMAL(5, 2)
	
	SET @Score = 0

	SELECT @Score = Score
	FROM dbo.DisciplineType WITH (NOLOCK)
	WHERE DisciplineTypeId = @DisciplineTypeId

	RETURN @Score
END