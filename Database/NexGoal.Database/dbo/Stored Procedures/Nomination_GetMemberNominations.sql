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
	SET NOCOUNT ON;

	SELECT EmployeeId
	INTO #member
	FROM dbo.func_GetMember(@ActorId) 
	WHERE ProcessId IN (3, 4, 5)

	SELECT pr.PerformanceRecordId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,pr.CreatorId
		,pr.EmployeeIds
		,pr.StatusId
		,pr.TypeId
		,ISNULL(n.StatusId, sysn.StatusId) AS NominationStatusId
		,ISNULL(n.EOQYId, sysn.EOQYId) AS NominationPeriodId
	INTO #nominationTbl
	FROM dbo.PerformanceRecord pr WITH (NOLOCK)
	LEFT JOIN dbo.NominationDetail nd WITH (NOLOCK) on pr.PerformanceRecordId = nd.PerformanceRecordId
	LEFT JOIN dbo.Nomination n WITH (NOLOCK) ON nd.NominationId = n.NominationId
	LEFT JOIN dbo.Nomination sysn WITH (NOLOCK) ON pr.PerformanceRecordId = sysn.PerformanceRecordId
	WHERE (pr.CreatorId = @ActorId AND nd.NominatorId = @ActorId
			AND (@MemberId IS NULL OR n.NomineeId = @MemberId)
			AND pr.TypeId = 3
		) 
		OR (pr.TypeId = 5 AND @MemberId IS NULL
			AND EXISTS (SELECT 1 FROM #member m WHERE sysn.NomineeId = m.EmployeeId)
			AND EXISTS (SELECT 1 FROM EOQY e WITH (NOLOCK) WHERE e.EOQYId = sysn.EOQYId AND e.EndDateForCommittee < GETDATE())
		)

	SELECT PerformanceRecordId
		,Comment
		,CreatedDate
		,ModifiedDate
		,CreatorId
		,EmployeeIds
		,StatusId
		,TypeId
		,NominationPeriodId
		,NominationStatusId
	FROM #nominationTbl
	ORDER BY CASE 
			WHEN StatusId = 2 THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT e.EOQYId AS 'NominationPeriodId'
		,e.[Quarter]
		,e.[Year]
		,e.StartDateForNominator
		,e.EndDateForNominator
		,e.EndDateForCommittee
	FROM dbo.EOQY e WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1 
			FROM #nominationTbl nt WHERE nt.NominationPeriodId = e.EOQYId
		)

	DROP TABLE #nominationTbl
	DROP TABLE #member
END