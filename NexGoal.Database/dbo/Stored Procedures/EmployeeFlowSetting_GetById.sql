
CREATE PROCEDURE [dbo].[EmployeeFlowSetting_GetById] @EmployeeId INT
	,@FlowTemplateId SMALLINT
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
		,ft.FlowTemplateId
		,ft.[Name] AS FlowTemplate
		,efs.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON efs.FlowTemplateId = ft.FlowTemplateId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON efs.EvaluatorId = emp.EmployeeId
	WHERE efs.EmployeeId = @EmployeeId
		AND efs.FlowTemplateId = @FlowTemplateId
	ORDER BY efs.ProcessId
		,efs.[Priority]
END