/****** Object:  Stored Procedure dbo.EPA_GetNumberOfRow    Script Date: 7/21/2016 2:45:45 PM ******/



CREATE PROCEDURE [dbo].[EPA_GetNumberOfRow] @EpaId INT
AS
/*
Created: 20160531@Razor.Nguyen
Task: Export PDF
DB: SMSEPA
Original:

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*)
	FROM [dbo].[EPADetail]
	WHERE ProcessId = 3
		AND EPAId = @EpaId
END
