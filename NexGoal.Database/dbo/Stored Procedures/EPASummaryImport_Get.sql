/****** Object:  Stored Procedure dbo.EPASummaryImport_Get    Script Date: 7/21/2016 2:45:54 PM ******/

CREATE PROCEDURE [dbo].[EPASummaryImport_Get]
AS
/*
Created: 20151002@Quinn.Vo
Task: Get EPA summary import to encode it.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[Id]
		,[OriginFinalScore]
		FROM [dbo].[EPASummaryImport] WITH (NOLOCK)
END
