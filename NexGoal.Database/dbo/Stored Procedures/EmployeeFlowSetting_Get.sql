
CREATE PROCEDURE [dbo].[EmployeeFlowSetting_Get] @FlowTemplateId SMALLINT
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
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON efs.FlowTemplateId = ft.FlowTemplateId
		AND ft.FlowTemplateId = @FlowTemplateId

	SELECT efs.EmployeeId
		,efs.ProcessId
		,efs.[Priority]
		,efs.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON efs.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON efs.FlowTemplateId = ft.FlowTemplateId
		AND ft.FlowTemplateId = @FlowTemplateId
	ORDER BY efs.EmployeeId
		,efs.ProcessId
		,efs.[Priority]
END