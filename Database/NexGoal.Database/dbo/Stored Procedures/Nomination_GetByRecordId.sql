CREATE PROCEDURE [dbo].[Nomination_GetByRecordId] 
	@RecordId INT
AS
/*
Created: 201801105@Elton.Ton
Task: Get member nomination detail.
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
		,pr.EmployeeId AS EmployeeIds
		,n.StatusId
		,pr.TypeId
		,n.StatusId AS NominationStatusId
		,n.EOQYId AS NominationPeriodId
	FROM dbo.PerformanceRecord pr WITH (NOLOCK)
	INNER JOIN dbo.NominationDetail nd WITH (NOLOCK) on pr.PerformanceRecordId = nd.PerformanceRecordId
	INNER JOIN dbo.Nomination n WITH (NOLOCK) ON nd.NominationId = n.NominationId
	WHERE pr.PerformanceRecordId = @RecordId AND pr.TypeId = 3

	SELECT e.EOQYId AS 'NominationPeriodId'
		,e.[Quarter]
		,e.[Year]
	FROM dbo.EOQY e WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1 
			FROM dbo.Nomination n WITH (NOLOCK)
			WHERE n.EOQYId = e.EOQYId
				AND EXISTS (
						SELECT 1 
						FROM dbo.NominationDetail nd WITH (NOLOCK)
						WHERE nd.PerformanceRecordId = @RecordId AND nd.NominationId = n.NominationId
					)
		)
END
