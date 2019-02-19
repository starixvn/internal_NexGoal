

CREATE PROCEDURE [dbo].[Nomination_GetMemberNomination] @ActorId INT
AS
/*
Created: 20181105@Elton.Ton
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
		,Comment NVARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,IsHost BIT
		,TypeId TINYINT
		,RoleId TINYINT
		,NominationPeriodId INT
		,NominationStatusId TINYINT
		)

	INSERT INTO #tblMember (MemberId)
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@ActorId)
	WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,EmployeeId
		,CreatorId
		,Comment
		,CreatedDate
		,ModifiedDate
		,IsHost
		,TypeId
		,RoleId
		,NominationPeriodId
		,NominationStatusId
		)
	SELECT pr.PerformanceRecordId
		,pr.EmployeeId
		,pr.CreatorId
		,pr.Comment
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
	FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON pr.PerformanceRecordId = nd.PerformanceRecordId
		AND nd.NominatorId = @ActorId
		INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON nd.NominationId = n.NominationId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
		AND period.StartDateForNominator <= @Today
	WHERE pr.CreatorId = @ActorId
		AND pr.TypeId = 3
	
	UNION
	SELECT pr.PerformanceRecordId
		,pr.EmployeeId
		,pr.CreatorId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,0 AS IsHost
		,pr.TypeId
		,pr.RoleId
		,period.EOQYId AS NominationPeriodId
		,n.StatusId AS NominationStatusId
	FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON pr.PerformanceRecordId = n.PerformanceRecordId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
		AND period.EndDateForCommittee < @Today
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON n.NomineeId = member.MemberId
	WHERE pr.TypeId = 5

	SELECT record.PerformanceRecordId
		,record.Comment
		,record.CreatedDate
		,record.ModifiedDate
		,record.IsHost
		,record.TypeId
		,record.RoleId
		,record.NominationStatusId
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