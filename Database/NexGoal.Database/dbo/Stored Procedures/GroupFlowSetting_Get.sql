
CREATE PROCEDURE [dbo].[GroupFlowSetting_Get]
AS
/*
Created: 20180830@Tim.Trinh
Task: get group flow setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT gr.GroupId
		,gr.Code
		,gr.[Name]
	FROM dbo.[Group] AS gr WITH (NOLOCK)
	WHERE gr.IsDeleted = 0
	ORDER BY gr.[Name]

	SELECT gfs.GroupId
		,gfs.ProcessId
		,gfs.[Priority]
		,gfs.EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gfs.EvaluatorId = emp.EmployeeId
	ORDER BY gfs.GroupId DESC
		,gfs.ProcessId
		,gfs.[Priority]
END