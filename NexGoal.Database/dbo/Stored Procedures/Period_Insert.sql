
CREATE PROCEDURE Period_Insert
	@Name NVARCHAR(200)
	,@TargetTypeId INT
	,@StartDate DATETIME
	,@EndDate DATETIME
	,@DisciplineStartDate DATETIME
	,@DisciplineEndDate DATETIME
	,@FocusStartDate DATETIME
	,@Description NVARCHAR(MAX)
	,@IsActive BIT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180727@Elton Ton
Task: Create period
DB: NexGoal
*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole AS er WITH (NOLOCK)
				INNER JOIN dbo.RoleFunction AS rf WITH (NOLOCK) ON er.RoleId = rf.RoleId
				AND rf.Policy = 2
				INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
				AND f.FunctionKey = 'EvaluationPeriod'
			WHERE er.EmployeeId = @CreatedBy
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You dont have permission'
		RETURN
	END

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML


	INSERT INTO [dbo].[Period]
           ([Name]
           ,[TargetTypeId]
           ,[Description]
           ,[StartDate]
           ,[EndDate]
           ,[DisciplineStartDate]
           ,[DisciplineEndDate]
           ,[FocusStartDate]
           ,[IsActive]
           ,[CreatedBy]
           ,[CreatedDate])
     VALUES
           (@Name
		   ,@TargetTypeId
		   ,@Description
		   ,@StartDate
		   ,@EndDate
		   ,@DisciplineStartDate
		   ,@DisciplineEndDate
		   ,@FocusStartDate
		   ,@IsActive
		   ,@CreatedBy
		   ,GETDATE())


	DECLARE @PeriodId INT = SCOPE_IDENTITY()

	SET @xml = (
			SELECT p.PeriodId AS '@pi'
			,p.Name AS '@n'
			,p.TargetTypeId AS '@tti'
			,p.Description AS '@d'
			,p.StartDate AS '@sd'
			,p.EndDate AS '@ed'
			,p.DisciplineStartDate AS '@dsd'
			,p.DisciplineEndDate AS '@ded'
			,p.FocusStartDate AS '@fsd'
			,p.IsActive AS '@ia'
			,p.CreatedBy AS '@cb'
			,p.CreatedDate AS '@cd'
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
			FOR XML PATH('r'), TYPE
		)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'Period_Insert'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@Duration = @Duration
END