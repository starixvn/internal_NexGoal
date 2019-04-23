CREATE PROCEDURE [dbo].[GroupFlowSetting_GetGroups]
AS
/*
Created: 20190422@Elton.ton
Task: get groups
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	SELECT gr.GroupId
		,gr.Code
		,gr.[Name]
		,(CASE
			WHEN EXISTS(SELECT 1 FROM dbo.GroupFlowSetting gfs WITH (NOLOCK) WHERE gfs.GroupId = gr.GroupId) THEN 1 
			ELSE 0 
		END ) AS 'HasSetting'
	FROM  dbo.[Group] AS gr WITH (NOLOCK)
	WHERE gr.IsDeleted = 0
END
