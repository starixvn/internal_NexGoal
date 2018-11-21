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

	SELECT pr.PerformanceRecordId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,pr.CreatorId
		,pr.EmployeeIds
		,pr.StatusId
		,pr.TypeId
		,n.StatusId AS NominationStatusId
		,n.EOQYId AS NominationPeriodId
	INTO #nominationTbl
	FROM dbo.PerformanceRecord pr WITH (NOLOCK)
	INNER JOIN dbo.NominationDetail nd WITH (NOLOCK) on pr.PerformanceRecordId = nd.PerformanceRecordId
	INNER JOIN dbo.Nomination n WITH (NOLOCK) ON nd.NominationId = n.NominationId
	WHERE pr.CreatorId = @ActorId AND nd.NominatorId = @ActorId
		AND (@MemberId IS NULL OR n.NomineeId = @MemberId)
	ORDER BY CASE 
			WHEN pr.StatusId = 2 THEN 1
			ELSE 2
			END
		,pr.CreatedDate DESC

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
END
