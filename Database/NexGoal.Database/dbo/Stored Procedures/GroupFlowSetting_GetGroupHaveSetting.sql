
CREATE PROCEDURE [dbo].[GroupFlowSetting_GetGroupHaveSetting]
AS
/*
Created: 20180803@Tim.Trinh
Task: get group have flow setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT gr.GroupId
		,gr.Code
		,gr.[Name]
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
END
