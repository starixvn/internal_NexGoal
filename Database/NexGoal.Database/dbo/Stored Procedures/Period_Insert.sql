
CREATE PROCEDURE [dbo].[Period_Insert] @Name NVARCHAR(200)
	,@TargetTypeId TINYINT
	,@StartDate DATE = NULL
	,@EndDate DATE = NULL
	,@DisciplineStartDate DATE = NULL
	,@DisciplineEndDate DATE = NULL
	,@FocusStartDate DATE = NULL
	,@FocusEndDate DATE = NULL
	,@Description NVARCHAR(MAX) = NULL
	,@IsActive BIT = 1
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180727@Elton.Ton
Task: Create period
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	IF EXISTS (
			SELECT 1
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.[Name] = @Name
				AND p.TargetTypeId = @TargetTypeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Period ' + @Name + ' has already existed.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Period p WITH (NOLOCK)
			WHERE @TargetTypeId = 1
				AND p.TargetTypeId = @TargetTypeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The Target Type Probation has already existed period.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.TargetTypeId = 2
				AND @TargetTypeId = 2
				AND @StartDate <= p.EndDate
				AND @EndDate >= p.StartDate
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The Start Date/End Date overlap on another period.'

		RETURN
	END

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	INSERT INTO [dbo].[Period] (
		[Name]
		,[TargetTypeId]
		,[Description]
		,[StartDate]
		,[EndDate]
		,[DisciplineStartDate]
		,[DisciplineEndDate]
		,[FocusStartDate]
		,[FocusEndDate]
		,[IsActive]
		,[CreatedBy]
		,[CreatedDate]
		)
	VALUES (
		@Name
		,@TargetTypeId
		,@Description
		,@StartDate
		,@EndDate
		,@DisciplineStartDate
		,@DisciplineEndDate
		,@FocusStartDate
		,@FocusEndDate
		,@IsActive
		,@CreatedBy
		,GETDATE()
		)

	DECLARE @PeriodId INT = SCOPE_IDENTITY()

	INSERT INTO [dbo].[PeriodProcessSetting] (
		[PeriodId]
		,[ProcessId]
		,[Duration]
		,[NoticeAtBegin]
		,[RemindDay]
		,[FocusDay]
		)
	SELECT @PeriodId
		,ProcessId
		,Duration
		,NoticeAtBegin
		,RemindDay
		,FocusDay
	FROM [dbo].[PeriodDefaultProcessSetting] WITH (NOLOCK)
	WHERE TargetTypeId = @TargetTypeId

	IF @FocusStartDate < @Today
	BEGIN
		EXEC dbo.PAFocus_Generate @PeriodId
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END

	SET @xml = (
			SELECT p.PeriodId AS '@pi'
				,p.[Name] AS '@n'
				,p.TargetTypeId AS '@tti'
				,p.[Description] AS '@d'
				,p.StartDate AS '@sd'
				,p.EndDate AS '@ed'
				,p.DisciplineStartDate AS '@dsd'
				,p.DisciplineEndDate AS '@ded'
				,p.FocusStartDate AS '@fsd'
				,p.FocusEndDate AS '@fed'
				,p.IsActive AS '@ia'
				,p.CreatedBy AS '@cb'
				,p.CreatedDate AS '@cd'
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
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
