


CREATE PROCEDURE [dbo].[Discipline_UpdateStatus] 
	@DisciplineId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(2000) = '' OUTPUT
AS
/*
Created: 20180716@Elton Ton
Task: update discipline status.
DB: NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
	,@endRun DATETIME
	,@duration INT
	,@logData NVARCHAR(MAX)
	,@logDataOld NVARCHAR(MAX)
	,@xml XML

	SET @startRun = GETDATE()

	SET @xml = (
		SELECT d.DisciplineId AS '@di'
		,d.IsActive AS '@ia'
		,d.ModifiedBy AS '@mb'
		,d.ModifiedDate AS '@md'
		FROM dbo.Discipline d WITH (NOLOCK)
		WHERE d.DisciplineId = @DisciplineId
		FOR XML PATH('r'),TYPE
	)

	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.Discipline WITH (ROWLOCK, UPDLOCK)
	SET IsActive = @IsActive
	,ModifiedBy = @ModifiedBy
	,ModifiedDate = GETDATE()
	WHERE DisciplineId = @DisciplineId

	SET @xml = (
		SELECT d.DisciplineId AS '@di'
		,d.IsActive AS '@ia'
		,d.ModifiedBy AS '@mb'
		,d.ModifiedDate AS '@md'
		FROM dbo.Discipline d WITH (NOLOCK)
		WHERE d.DisciplineId = @DisciplineId
		FOR XML PATH('r'),TYPE
	)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
	,@SPName = 'Discipline_UpdateStatus'
	,@ObjectId = @DisciplineId
	,@Data = @logData
	,@OldData = @logDataOld
	,@Duration = @Duration
END