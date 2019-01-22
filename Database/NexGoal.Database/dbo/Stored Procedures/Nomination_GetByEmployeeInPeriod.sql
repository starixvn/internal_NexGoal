CREATE PROCEDURE [dbo].[Nomination_GetByEmployeeInPeriod] @EmployeeId INT
	,@PeriodId INT
AS
/*
Created: 20190122@Oliver.Dang
Task: Get achievements by employee in period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @FromDate DATE
			,@ToDate DATE
			,@Draft TINYINT = 4

	SELECT @FromDate = p.StartDate
		  ,@ToDate = p.EndDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT eoqy.EOQYId AS NominationPeriodId
		,nomination.StatusId
		,eoqy.EndDateForCommittee
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
		AND nomination.StatusId <> @Draft
		INNER JOIN dbo.EOQY AS eoqy WITH (NOLOCK) ON nomination.EOQYId = eoqy.EOQYId
		AND eoqy.EndDateForCommittee BETWEEN @FromDate AND @ToDate
	WHERE emp.EmployeeId = @EmployeeId
	ORDER BY eoqy.EndDateForCommittee DESC

	SELECT eoqy.EOQYId AS NominationPeriodId
		,eoqy.[Quarter]
		,eoqy.[Year]
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON eoqy.EOQYId = nomination.EOQYId
		AND nomination.StatusId <> @Draft
	WHERE nomination.NomineeId = @EmployeeId
		AND eoqy.EndDateForCommittee BETWEEN @FromDate AND @ToDate
END