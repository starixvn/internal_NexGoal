
CREATE PROCEDURE [dbo].[Nomination_GetMemberNominations] @ActorId INT
	,@MemberId INT = NULL
AS
/*
Created: 201801105@Elton.Ton
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
		-- record status
		,@Submited TINYINT = 3
		,@Closed TINYINT = 4

	CREATE TABLE #tblMember (MemberId INT)

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,EmployeeIds VARCHAR(MAX)
		,CreatorId INT
		,Comment NVARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,StatusId TINYINT
		,TypeId TINYINT
		,RoleId TINYINT
		,NominationPeriodId TINYINT
		,NominationStatusId TINYINT
		)

	INSERT INTO #tblMember (MemberId)
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@ActorId)
	WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		AND (
			@MemberId IS NULL
			OR EmployeeId = @MemberId
			)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,EmployeeIds
		,CreatorId
		,Comment
		,CreatedDate
		,ModifiedDate
		,StatusId
		,TypeId
		,RoleId
		,NominationPeriodId
		,NominationStatusId
		)
	SELECT pr.PerformanceRecordId
		,pr.EmployeeIds
		,pr.CreatorId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,(CASE 
			WHEN @Today BETWEEN period.StartDateForNominator
					AND period.EndDateForNominator THEN @Submited
			ELSE @Closed END
			) AS StatusId
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
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON n.NomineeId = member.MemberId
	WHERE pr.CreatorId = @ActorId
		AND pr.TypeId = 3
	
	UNION
	SELECT pr.PerformanceRecordId
		,pr.EmployeeIds
		,pr.CreatorId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,@Closed AS StatusId
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

	SELECT PerformanceRecordId
		,Comment
		,CreatedDate
		,ModifiedDate
		,CreatorId
		,EmployeeIds
		,StatusId
		,TypeId
		,RoleId
		,NominationPeriodId
		,NominationStatusId
	FROM #tblPerformanceRecord WITH (NOLOCK)
	ORDER BY CASE 
			WHEN StatusId = 2 THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT e.EOQYId AS NominationPeriodId
		,e.[Quarter]
		,e.[Year]
		,e.StartDateForNominator
		,e.EndDateForNominator
		,e.EndDateForCommittee
	FROM dbo.EOQY AS e WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #tblPerformanceRecord AS tbl WITH (NOLOCK)
			WHERE tbl.NominationPeriodId = e.EOQYId
			)

	DROP TABLE #tblPerformanceRecord

	DROP TABLE #tblMember
END