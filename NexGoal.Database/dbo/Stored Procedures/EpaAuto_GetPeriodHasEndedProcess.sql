


CREATE PROCEDURE [dbo].[EpaAuto_GetPeriodHasEndedProcess] @PeriodId INT = NULL
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Get period has employee that started evaluation.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT pe.PeriodId
		
	FROM dbo.Period AS pe WITH (NOLOCK) 
	WHERE CAST(pe.EndDate AS DATE) = CAST(GETDATE() AS DATE)
		AND (
			ISNULL(@PeriodId, '') = ''
			OR pe.PeriodId = @PeriodId
			)
END