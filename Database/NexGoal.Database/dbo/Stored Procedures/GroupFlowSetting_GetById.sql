
CREATE PROCEDURE [dbo].[GroupFlowSetting_GetById] @GroupId INT
	,@FlowTemplateId SMALLINT
AS
/*
Created: 20180802@Tim.Trinh
Task: get group flow setting by id
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT gfs.GroupId
		,gfs.ProcessId
		,gfs.[Priority]
		,ft.FlowTemplateId
		,ft.[Name] AS FlowTemplate
		,gfs.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON gfs.FlowTemplateId = ft.FlowTemplateId
		AND ft.FlowTemplateId = @FlowTemplateId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gfs.EvaluatorId = emp.EmployeeId
	WHERE gfs.GroupId = @GroupId
	ORDER BY gfs.ProcessId
		,gfs.[Priority]
END
