
CREATE PROCEDURE [dbo].[Discipline_Insert] @EmployeeId INT
	,@EffectiveDate DATE
	,@Description NVARCHAR(max)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180716@Elton.Ton
Task: inser discipline
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
     
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	INSERT INTO [dbo].[Discipline] (
		[EmployeeId]
		,[Description]
		,[IsActive]
		,[EffectiveDate]
		,[CreatedBy]
		,[CreatedDate]
		)
	VALUES (
		@EmployeeId
		,@Description
		,1
		,@EffectiveDate
		,@CreatedBy
		,GETDATE()
		)

	DECLARE @DisciplineId INT = SCOPE_IDENTITY()

	SET @xml = (
			SELECT d.DisciplineId AS '@di'
				,d.EmployeeId AS '@ei'
				,d.[Description] AS '@des'
				,d.EffectiveDate AS '@ed'
				,d.CreatedBy AS '@cb'
				,d.CreatedDate AS '@cd'
			FROM dbo.Discipline d WITH (NOLOCK)
			WHERE d.DisciplineId = @DisciplineId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'Discipline_Insert'
		,@ObjectId = @DisciplineId
		,@Data = @logData
		,@Duration = @Duration
END
