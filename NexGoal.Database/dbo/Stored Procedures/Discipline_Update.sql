
CREATE PROCEDURE [dbo].[Discipline_Update] @DisciplineId INT
	,@EffectiveDate DATE
	,@Description NVARCHAR(max)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180716@Elton.Ton
Task: update discipline
DB: bobd_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT d.DisciplineId AS '@di'
				,d.[Description] AS '@des'
				,d.EffectiveDate AS '@ed'
				,d.ModifiedBy AS '@mb'
				,d.ModifiedDate AS '@md'
			FROM dbo.Discipline d WITH (NOLOCK)
			WHERE d.DisciplineId = @DisciplineId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.Discipline WITH (ROWLOCK, UPDLOCK)
	SET EffectiveDate = @EffectiveDate
		,[Description] = @Description
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE DisciplineId = @DisciplineId

	SET @xml = (
			SELECT d.DisciplineId AS '@di'
				,d.[Description] AS '@des'
				,d.EffectiveDate AS '@ed'
				,d.ModifiedBy AS '@mb'
				,d.ModifiedDate AS '@md'
			FROM dbo.Discipline d WITH (NOLOCK)
			WHERE d.DisciplineId = @DisciplineId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Discipline_Update'
		,@ObjectId = @DisciplineId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END
