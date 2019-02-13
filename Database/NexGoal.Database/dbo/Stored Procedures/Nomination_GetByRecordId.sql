
CREATE PROCEDURE [dbo].[Nomination_GetByRecordId] @RecordId INT
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
	
	DECLARE @Today DATE = GETDATE()

	SELECT pr.PerformanceRecordId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,(CASE 
			WHEN @Today BETWEEN period.StartDateForNominator
					AND period.EndDateForNominator THEN 1
			ELSE 0 END
			) AS IsHost
		,nomination.StatusId AS NominationStatusId
		,pr.TypeId
		,nominee.EmployeeId
		,nominee.NickName
		,nominee.FullName
		,nominee.Code
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
		INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON pr.PerformanceRecordId = nd.PerformanceRecordId
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON nd.NominationId = nomination.NominationId
		INNER JOIN dbo.Employee AS nominee WITH (NOLOCK) ON pr.EmployeeId = nominee.EmployeeId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON pr.CreatorId = creator.EmployeeId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
	WHERE pr.PerformanceRecordId = @RecordId
		AND pr.TypeId = 3
END
