

/****** Object:  Stored Procedure dbo.EpaPeriod_GetAllPeriodIncludeImport    Script Date: 7/21/2016 2:45:53 PM ******/
/****** Object:  Stored Procedure dbo.EpaPeriod_GetAllPeriodIncludeImport    Script Date: 4/6/2016 11:27:45 AM ******/
CREATE PROCEDURE [dbo].[EpaPeriod_GetAllPeriodIncludeImport]
AS
/*
Created: 2015-10-19@Josh.Phan
Task: Get all period, inlcude data import.
DB: SMSEPA
Original: 

Revisions:  
20160524@Razor.Nguyen: Exclude Monthly EPA
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT p.[PeriodId]
		,p.[Name]
		,p.[StartDate]
		,p.[EndDate]
		,tg.TargetTypeId
		,tg.Name
	FROM [dbo].[Period] AS p WITH (NOLOCK)
		INNER JOIN dbo.TargetType AS tg WITH (NOLOCK) ON tg.TargetTypeId = p.TargetTypeId
	WHERE p.[IsActive] = 1
		AND p.[TargetTypeId] != 2
	ORDER BY p.[EndDate] DESC
END
