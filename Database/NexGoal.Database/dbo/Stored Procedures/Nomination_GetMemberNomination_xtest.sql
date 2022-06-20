CREATE PROCEDURE [dbo].[Nomination_GetMemberNomination_xtest] @ActorId INT
AS
/*
Created: 20222006@Otis.Vu
Task: Get member nomination details.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/

BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		-- process
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		-- nomination status
		,@Nominated TINYINT = 1
		,@Shortlisted TINYINT = 2
		,@Winner TINYINT = 3

	CREATE TABLE #tblMember (MemberId INT)

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,EmployeeId INT
		,CreatorId INT
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,IsHost BIT
		,TypeId TINYINT
		,RoleId TINYINT
		,NominationPeriodId INT
		,NominationStatusId TINYINT
		,KPIGroupId INT
		,Rate INT
		,KPIId INT
		,Comment NVARCHAR(MAX)
		)

	INSERT INTO #tblMember (MemberId)
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@ActorId)
	WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,EmployeeId
		,CreatorId
		,CreatedDate
		,ModifiedDate
		,IsHost
		,TypeId
		,RoleId
		,NominationPeriodId
		,NominationStatusId
		,KPIGroupId
		,Rate
		,KPIId
		,Comment
		)
	SELECT pr.PerformanceRecordId
		,pr.EmployeeId
		,pr.CreatorId
		,pr.CreatedDate
		,pr.ModifiedDate
		,(CASE 
			WHEN @Today BETWEEN period.StartDateForNominator
					AND period.EndDateForNominator THEN 1
			ELSE 0 END
			) AS IsHost
		,pr.TypeId
		,pr.RoleId
		,period.EOQYId AS NominationPeriodId
		,n.StatusId AS NominationStatusId
		,nkg.KPIGroupId
		,nkg.Rate
		,nk.KPIId
		,nk.Comment
	FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON pr.PerformanceRecordId = nd.PerformanceRecordId
		AND nd.NominatorId = @ActorId
		INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON nd.NominationId = n.NominationId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
		AND period.StartDateForNominator <= @Today
		INNER JOIN dbo.NominationKPIGroup as nkg ON nd.NominationId = nkg.NominationId 
		AND nd.NominatorId = nkg.NominatorId
		INNER JOIn dbo.NominationKPI as nk ON nkg.KPIGroupId = nk.KPIGroupId 
		AND nkg.NominatorId = nk.NominatorId AND nkg.NominationId = nk.NominationId
	WHERE pr.CreatorId = @ActorId
		AND pr.TypeId = 3
		AND pr.CreatorId = nkg.NominatorId
	
	UNION
	SELECT pr.PerformanceRecordId
		,pr.EmployeeId
		,pr.CreatorId
		,pr.CreatedDate
		,pr.ModifiedDate
		,0 AS IsHost
		,pr.TypeId
		,pr.RoleId
		,period.EOQYId AS NominationPeriodId
		,n.StatusId AS NominationStatusId
		,nkg.KPIGroupId
		,nkg.Rate
		,nk.KPIId
		,nk.Comment
	FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON pr.PerformanceRecordId = n.PerformanceRecordId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
		AND period.EndDateForCommittee < @Today
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON n.NomineeId = member.MemberId
		INNER JOIN dbo.NominationKPIGroup as nkg ON n.NominationId = nkg.NominationId
		INNER JOIn dbo.NominationKPI as nk ON nkg.KPIGroupId = nk.KPIGroupId
		AND nkg.NominatorId = nk.NominatorId AND nkg.NominationId = nk.NominationId
	WHERE pr.TypeId = 5


	SELECT record.PerformanceRecordId
		,record.CreatedDate
		,record.ModifiedDate
		,record.IsHost
		,record.TypeId
		,record.RoleId
		,record.NominationStatusId
		,record.KPIGroupId
		,record.Rate
		,record.KPIId
		,record.Comment
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,record.CreatorId AS EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
		,record.NominationPeriodId
		,period.[Quarter]
		,period.[Year]
		,period.StartDateForNominator
		,period.EndDateForNominator
		,period.EndDateForCommittee
	FROM #tblPerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON record.NominationPeriodId = period.EOQYId
	ORDER BY record.CreatedDate DESC
		--,CASE record.NominationStatusId
		--	WHEN @Winner THEN 1
		--	WHEN @Shortlisted THEN 2
		--	WHEN @Nominated THEN 3
		--	ELSE 4
		--	END

	DROP TABLE #tblPerformanceRecord

	DROP TABLE #tblMember
END
