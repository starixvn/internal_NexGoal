
CREATE PROCEDURE [dbo].[FlowTemplate_GetById] @FlowTemplateId SMALLINT
AS
/*
Created: 20180730@Tim.Trinh
Task: Get flow template by id
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ft.FlowTemplateId
		,ft.[Name]
		,ft.[Description]
		,ft.IsActive
		,rr.ResourceId AS TargetTypeId
		,rr.Value AS TargetTypeName
	FROM dbo.FlowTemplate AS ft WITH (NOLOCK)
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON ft.TargetTypeId = rr.ResourceId
		AND rr.RefName = 'TargetType'
	WHERE ft.FlowTemplateId = @FlowTemplateId
END
