CREATE PROCEDURE [dbo].[PMSetting_Insert]
	@ProjectManagerId INT
	,@MemberIds VARCHAR(MAX)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190107@Elton.Ton
Task: add PM setting
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT pms.ProjectManagerId AS '@pmi'
				,pms.MemberId AS '@mi'
			FROM dbo.PMSetting pms WITH (NOLOCK)
			WHERE pms.ProjectManagerId = @ProjectManagerId
			FOR XML PATH('r')
				,TYPE
			)

	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	INSERT INTO dbo.PMSetting (
		ProjectManagerId
		,MemberId
		)
	SELECT @ProjectManagerId
		,val
	FROM dbo.func_SplitString(@MemberIds, ',')

	SET @xml = (
			SELECT pms.ProjectManagerId AS '@pmi'
				,pms.MemberId AS '@mi'
			FROM dbo.PMSetting pms WITH (NOLOCK)
			WHERE pms.ProjectManagerId = @ProjectManagerId
			FOR XML PATH('r')
				,TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'PMSetting_Insert'
		,@ObjectId = @ProjectManagerId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

END

GO


