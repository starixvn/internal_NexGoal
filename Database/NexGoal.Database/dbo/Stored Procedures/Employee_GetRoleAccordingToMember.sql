
CREATE PROCEDURE [dbo].[Employee_GetRoleAccordingToMember] @ActorId INT
	,@MemberId INT
AS
/*
Created: 20181214@Tim.Trinh
Task: get role according to member
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RoleId TINYINT
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT @RoleId = CASE MAX(data.[Priority])
			WHEN 3 THEN @EvaluatorProcessId
			WHEN 2 THEN @ReviewerProcessId
			ELSE @PMProcessId
			END
	FROM (
		SELECT EmployeeId
			,(CASE 
				WHEN ProcessId = @EvaluatorProcessId THEN 3
				WHEN ProcessId = @ReviewerProcessId THEN 2
				ELSE 1 END
				) AS [Priority]
		FROM dbo.func_GetMember(@ActorId)
		WHERE EmployeeId = @MemberId
			AND ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		) AS data
	GROUP BY data.EmployeeId

	SELECT ISNULL(@RoleId, 0)
END