
CREATE PROCEDURE [dbo].[FlowTemplate_UpdateStatus] @FlowTemplateId SMALLINT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180801@Tim.Trinh
Task: update status flow template
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
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- Write log before update	
	SET @xml = (
			SELECT FlowTemplateId AS '@id'
				,IsActive AS '@status'
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE FlowTemplateId = @FlowTemplateId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.FlowTemplate WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
	WHERE FlowTemplateId = @FlowTemplateId

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Fail to update status of flow template.'

		RETURN
	END

	-- write log after update
	SET @xml = (
			SELECT FlowTemplateId AS '@id'
				,IsActive AS '@status'
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE FlowTemplateId = @FlowTemplateId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'FlowTemplate_UpdateStatus'
		,@ObjectId = @FlowTemplateId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END