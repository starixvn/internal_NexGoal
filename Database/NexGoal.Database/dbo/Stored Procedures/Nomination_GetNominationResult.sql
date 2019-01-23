
CREATE PROCEDURE [dbo].[Nomination_GetNominationResult] @EmployeeId INT
AS
/*
Created: 20180927@Tim.Trinh
Task: Get nomination result.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()

	SELECT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,nomination.StatusId
		,eoqy.EOQYId AS NominationPeriodId
		,eoqy.[Quarter]
		,eoqy.[Year]
		,eoqy.StartDateForNominator
		,eoqy.EndDateForNominator
		,eoqy.EndDateForCommittee
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
		INNER JOIN dbo.EOQY AS eoqy WITH (NOLOCK) ON nomination.EOQYId = eoqy.EOQYId
		AND eoqy.EndDateForCommittee < @Today
	WHERE emp.EmployeeId = @EmployeeId
	ORDER BY eoqy.EndDateForCommittee DESC
END