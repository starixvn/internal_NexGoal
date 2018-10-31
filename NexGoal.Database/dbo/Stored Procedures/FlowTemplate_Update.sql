
CREATE PROCEDURE [dbo].[FlowTemplate_Update] @FlowTemplateId SMALLINT
	,@Name NVARCHAR(200)
	,@TargetTypeId INT
	,@Description NVARCHAR(500) = NULL
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180730@Tim.Trinh
Task: update flow template
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE [Name] = @Name
				AND FlowTemplateId != @FlowTemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Flow Template ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE TargetTypeId = @TargetTypeId
				AND FlowTemplateId != @FlowTemplateId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This Target Type has already existed setting.'

		RETURN
	END

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- write log before update
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

	UPDATE dbo.FlowTemplate WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,TargetTypeId = @TargetTypeId
		,[Description] = @Description
		,IsActive = @IsActive
	WHERE FlowTemplateId = @FlowTemplateId

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Fail to update information of flow template.'

		RETURN
	END

	-- write log after update
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
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'FlowTemplate_Update'
		,@ObjectId = @FlowTemplateId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END