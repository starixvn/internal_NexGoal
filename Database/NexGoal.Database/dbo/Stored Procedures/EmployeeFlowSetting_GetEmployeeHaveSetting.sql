
CREATE PROCEDURE [dbo].[EmployeeFlowSetting_GetEmployeeHaveSetting] @GroupId INT = NULL
AS
/*
Created: 20150807@Tim.Trinh
Task: get employee have flow setting
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
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.EmployeeFlowSetting AS efs WITH (NOLOCK) ON emp.EmployeeId = efs.EmployeeId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND ISNULL(@GroupId, emp.GroupId) = emp.GroupId
	ORDER BY emp.NickName
END
