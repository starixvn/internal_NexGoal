CREATE PROCEDURE [dbo].[Nomination_GetByPeriodId] 
	@NominationPeriodId INT
AS
/*
Created: 20181119@Elton.Ton
Task: get nomination period.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT n.NominationId
		,n.EOQYId AS 'NominationPeriodId'
		,n.NomineeId
		,n.StatusId
		,n.PerformanceRecordId
	FROM dbo.Nomination n WITH (NOLOCK)
	WHERE n.EOQYId = @NominationPeriodId

	SELECT pr.PerformanceRecordId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,pr.CreatorId
		,pr.EmployeeIds
		,pr.StatusId
	FROM dbo.PerformanceRecord pr WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.NominationDetail nd WITH (NOLOCK)
				INNER JOIN dbo.Nomination n WITH (NOLOCK) ON nd.NominationId = n.NominationId
			WHERE nd.PerformanceRecordId = pr.PerformanceRecordId AND n.EOQYId = @NominationPeriodId
	)
END
