
CREATE PROCEDURE [dbo].[FlowTemplate_Insert] @Name NVARCHAR(200)
	,@TargetTypeId TINYINT
	,@Description NVARCHAR(500) = NULL
	,@IsActive BIT = 1
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180730@Tim.Trinh
Task: insert flow template
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FlowTemplateId SMALLINT
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF EXISTS (
			SELECT 1
			FROM dbo.FlowTemplate WITH (NOLOCK)
			WHERE [Name] = @Name
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
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'This Target Type has already existed setting.'

		RETURN
	END

	INSERT INTO dbo.FlowTemplate (
		[Name]
		,TargetTypeId
		,[Description]
		,IsActive
		)
	VALUES (
		@Name
		,@TargetTypeId
		,@Description
		,@IsActive
		)

	SET @FlowTemplateId = SCOPE_IDENTITY()

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to insert flow template.'

		RETURN
	END

	-- write log after insert
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

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'FlowTemplate_Insert'
		,@ObjectId = @FlowTemplateId
		,@Data = @logData
		,@Duration = @Duration
END