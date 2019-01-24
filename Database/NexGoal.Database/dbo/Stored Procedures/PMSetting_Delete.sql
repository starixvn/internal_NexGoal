
CREATE PROCEDURE [dbo].[PMSetting_Delete] @ProjectManagerId INT
	,@MemberId INT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190107@Elton.Ton
Task: delete PM setting
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
			SELECT ProjectManagerId AS '@pmi'
				,MemberId AS '@mi'
			FROM dbo.PMSetting WITH (NOLOCK)
			WHERE ProjectManagerId = @ProjectManagerId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE
	FROM dbo.PMSetting
	WHERE ProjectManagerId = @ProjectManagerId
		AND MemberId = @MemberId

	SET @xml = (
			SELECT ProjectManagerId AS '@pmi'
				,MemberId AS '@mi'
			FROM dbo.PMSetting WITH (NOLOCK)
			WHERE ProjectManagerId = @ProjectManagerId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'PMSetting_Delete'
		,@ObjectId = @ProjectManagerId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END

GO


