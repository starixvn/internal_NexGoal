
CREATE PROCEDURE [dbo].[FlowTemplate_Delete] @FlowTemplateId SMALLINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180730@Tim.Trinh
Task: delete flow template
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	
	-- Write log before delete	
	SET @xml = (
			SELECT FlowTemplateId AS '@id'
				,[Name] AS '@n'
				,TargetTypeId AS '@ttid'
				,[Description] AS '@d'
				,IsActive AS '@status'
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE FlowTemplateId = @FlowTemplateId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE
	FROM dbo.FlowTemplate
	WHERE FlowTemplateId = @FlowTemplateId
	
	-- write log after delete
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'FlowTemplate_Delete'
		,@ObjectId = @FlowTemplateId
		,@OldData = @logDataOld
		,@Duration = @Duration
END