CREATE PROCEDURE [dbo].[Probation_GetProbationersNotInEvaluation]
AS
/*
Created: 20190702@Elton.ton
Task: get probationers not in evaluation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
		,e.FullName
		,e.NickName
	FROM dbo.Employee e WITH (NOLOCK)
	WHERE e.ConfirmedDate IS NULL AND e.Enabled = 1 AND e.IsDeleted = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.ProbationSetting ps WITH (NOLOCK)
			WHERE e.EmployeeId = ps.EmployeeId
		)
END