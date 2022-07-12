CREATE PROCEDURE [dbo].[Nomination_GetMemberNominationById_xtest] @ActorId INT
	,@MemberId INT
AS
/*
Created: 20201207@Otis.Vu
Task: Get member nomination details.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		
	SELECT pr.PerformanceRecordId
		,pr.EmployeeId AS EmployeeIds
		,pr.CreatedDate
		,pr.ModifiedDate
		,(CASE 
			WHEN @Today BETWEEN period.StartDateForNominator
					AND period.EndDateForNominator THEN 1
			ELSE 0 END
			) AS IsHost
		,pr.TypeId
		,pr.RoleId
		,n.StatusId AS NominationStatusId
		,nkg.KPIGroupId
		,nkg.Rate
		,nk.KPIId
		,nk.Comment
		,kpig.Name
		,kpi.Name AS KPIName
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
		,period.EOQYId AS NominationPeriodId
		,period.[Quarter]
		,period.[Year]
		,period.StartDateForNominator
		,period.EndDateForNominator
		,period.EndDateForCommittee
	FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON pr.CreatorId = creator.EmployeeId
		AND creator.EmployeeId = @ActorId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmployeeId = @MemberId
		AND pr.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON pr.PerformanceRecordId = nd.PerformanceRecordId
		AND nd.NominatorId = @ActorId
		INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON nd.NominationId = n.NominationId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
		AND period.StartDateForNominator <= @Today
		INNER JOIN dbo.NominationKPIGroup AS nkg ON nd.NominationId = nkg.NominationId 
		AND nd.NominatorId = nkg.NominatorId
		INNER JOIn dbo.NominationKPI AS nk ON nkg.KPIGroupId = nk.KPIGroupId 
		AND nkg.NominatorId = nk.NominatorId AND nkg.NominationId = nk.NominationId
		INNER JOIN dbo.KPI AS kpi ON kpi.KPIId = nk.KPIId AND kpi.KPIGroupId = nk.KPIGroupId
		INNER JOIN dbo.KPIGroup AS kpig ON kpig.KPIGroupId = kpi.KPIGroupId
	WHERE pr.TypeId = 3 -- nomination
	ORDER BY CreatedDate DESC
END

