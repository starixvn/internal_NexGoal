
CREATE PROCEDURE [dbo].[EmployeeFlowSetting_GetById] @EmployeeId INT
AS
/*
Created: 20180807@Tim.Trinh
Task: get employee flow setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT efs.EmployeeId
		,efs.ProcessId
		,efs.[Priority]
		,efs.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON efs.EvaluatorId = emp.EmployeeId
	WHERE efs.EmployeeId = @EmployeeId
	ORDER BY efs.ProcessId
		,efs.[Priority]
END
