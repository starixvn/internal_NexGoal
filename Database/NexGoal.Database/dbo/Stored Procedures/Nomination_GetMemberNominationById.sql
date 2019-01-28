

CREATE PROCEDURE [dbo].[Nomination_GetMemberNominationById] @ActorId INT
	,@MemberId INT
AS
/*
Created: 20190118@Tim.Trinh
Task: Get member nomination details.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		-- record status
		,@Submited TINYINT = 3
		,@Closed TINYINT = 4
		
	SELECT pr.PerformanceRecordId
		,pr.EmployeeId AS EmployeeIds
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
		,n.StatusId AS NominationStatusId
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
	WHERE pr.TypeId = 3 -- nomination
	ORDER BY CreatedDate DESC
END