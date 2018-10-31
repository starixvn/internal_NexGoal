
CREATE PROCEDURE [dbo].[GroupFlowSetting_Delete] @GroupId INT
	,@FlowTemplateId SMALLINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180802@Tim.Trinh
Task: delete group flow setting
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
		,@logDataOld NVARCHAR(MAX) = NULL
		,@xml XML

	SET @xml = (
			SELECT @FlowTemplateId AS '@fid'
				,@GroupId AS '@gid'
				,(
					SELECT ProcessId AS '@pid'
						,EvaluatorId AS '@evaid'
						,[Priority] AS '@p'
					FROM dbo.GroupFlowSetting WITH (NOLOCK)
					WHERE GroupId = @GroupId
						AND FlowTemplateId = @FlowTemplateId
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE
	FROM dbo.GroupFlowSetting
	WHERE GroupId = @GroupId
		AND FlowTemplateId = @FlowTemplateId

	-- write log after delete
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'GroupFlowSetting_Delete'
		,@ObjectId = @GroupId
		,@OldData = @logDataOld
		,@Duration = @Duration
END