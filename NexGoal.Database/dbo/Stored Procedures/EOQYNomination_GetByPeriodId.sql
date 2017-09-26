
CREATE PROCEDURE [dbo].[EOQYNomination_GetByPeriodId] @PeriodId INT
	,@NomineeId INT
AS
/*
Created: 20160907@Razor.Nguyen
Task: Get nomination by NominatorId.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT nt.StatusId
		,nt.EOQYId
		,eoq.[Quarter]
		,eoq.[Year]
		,COUNT(*) AS NumberVotes
	FROM dbo.EOQYNomination AS nt WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON nt.EOQYId = eoq.EOQYId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.AttendanceStartDate <= eoq.EndDate
		AND p.AttendanceEndDate >= eoq.EndDate
	WHERE eoq.StartDate < CAST(GETDATE() AS DATE)
		AND eoq.EndDate < CAST(GETDATE() AS DATE)
		AND p.PeriodId = @PeriodId
		AND nt.NomineeId = @NomineeId
		AND eoq.IsQuarter = 1
	GROUP BY nt.StatusId
		,nt.EOQYId
		,eoq.[Quarter]
		,eoq.[Year]
	
	UNION
	SELECT DISTINCT nt.StatusId
		,nt.EOQYId
		,eoq.[Quarter]
		,eoq.[Year]
		,COUNT(*) AS NumberVotes
	FROM dbo.EOQYNomination AS nt WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON nt.EOQYId = eoq.EOQYId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON 1 = 1
	WHERE eoq.StartDate < CAST(GETDATE() AS DATE)
		AND eoq.EndDate < CAST(GETDATE() AS DATE)
		AND p.PeriodId = @PeriodId
		AND nt.NomineeId = @NomineeId
		AND eoq.IsQuarter = 0
		AND YEAR(eoq.StartDate) IN (Year(p.StartDate), Year(p.StartDate) - 1)
		
	GROUP BY nt.StatusId
		,nt.EOQYId
		,eoq.[Quarter]
		,eoq.[Year]
END