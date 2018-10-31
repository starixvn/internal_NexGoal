
CREATE PROCEDURE [dbo].[Employee_GetNominationResult] @EmployeeId INT
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
		,eoqy.EOQYId AS NominationPeriodId
		,nomination.StatusId
		,eoqy.EndDateForCommittee
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
		INNER JOIN dbo.EOQY AS eoqy WITH (NOLOCK) ON nomination.EOQYId = eoqy.EOQYId
		AND eoqy.EndDateForCommittee < @Today
	WHERE emp.EmployeeId = @EmployeeId
	ORDER BY eoqy.EndDateForCommittee DESC

	SELECT eoqy.EOQYId AS NominationPeriodId
		,eoqy.[Quarter]
		,eoqy.[Year]
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON eoqy.EOQYId = nomination.EOQYId
	WHERE nomination.NomineeId = @EmployeeId
		AND eoqy.EndDateForCommittee < @Today
END