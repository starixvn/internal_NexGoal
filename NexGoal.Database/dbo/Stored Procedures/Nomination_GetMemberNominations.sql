CREATE PROCEDURE [dbo].[Nomination_GetMemberNominations] 
	@ActorId INT
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
	DECLARE @PMProcessId TINYINT = 3, @EvaluatorProcessId TINYINT = 4, @ReviewerProcessId TINYINT = 5

	SELECT EmployeeId
	INTO #tblMember
	FROM dbo.func_GetMember(@ActorId)
	WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)

	SELECT pr.PerformanceRecordId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,pr.CreatorId
		,pr.EmployeeIds
		,pr.StatusId
		,n.NomineeId
		,n.NominationId
	INTO #nominationTbl
	FROM dbo.PerformanceRecord pr WITH (NOLOCK)
	INNER JOIN dbo.NominationDetail nd WITH (NOLOCK) on pr.PerformanceRecordId = nd.PerformanceRecordId
	INNER JOIN dbo.Nomination n WITH (NOLOCK) ON nd.NominationId = n.NominationId
	WHERE EXISTS (SELECT 1 FROM #tblMember m WHERE m.EmployeeId = n.NomineeId)
		AND pr.CreatorId = @ActorId AND nd.NominatorId = @ActorId
		AND (@MemberId IS NULL OR n.NomineeId = @MemberId)
	ORDER BY pr.CreatedDate DESC

	SELECT PerformanceRecordId
		,Comment
		,CreatedDate
		,ModifiedDate
		,CreatorId
		,EmployeeIds
		,StatusId
		,NominationId
	FROM #nominationTbl

	SELECT n.NominationId
		,n.EOQYId AS 'NominationPeriodId'
		,n.StatusId
		,n.NomineeId
	FROM dbo.Nomination n WITH (NOLOCK)
	WHERE EXISTS (SELECT 1 FROM #nominationTbl nt WHERE nt.NominationId = n.NominationId AND nt.NomineeId = n.NomineeId)

	SELECT e.EOQYId AS 'NominationPeriodId'
		,e.[Quarter]
		,e.[Year]
	FROM dbo.EOQY e WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1 
			FROM dbo.Nomination n WITH (NOLOCK)
			WHERE n.EOQYId = e.EOQYId
				AND EXISTS (SELECT 1 FROM #nominationTbl nt WHERE nt.NominationId = n.NominationId AND nt.NomineeId = n.NomineeId)
		)

	DROP TABLE #nominationTbl
	DROP TABLE #tblMember
END