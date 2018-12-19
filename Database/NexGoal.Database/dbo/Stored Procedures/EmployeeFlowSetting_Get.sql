
CREATE PROCEDURE [dbo].[EmployeeFlowSetting_Get]
AS
/*
Created: 20180830@Tim.Trinh
Task: get employee flow setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON efs.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0

	SELECT efs.EmployeeId
		,efs.ProcessId
		,efs.[Priority]
		,efs.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON efs.EvaluatorId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
	ORDER BY efs.EmployeeId
		,efs.ProcessId
		,efs.[Priority]
END
