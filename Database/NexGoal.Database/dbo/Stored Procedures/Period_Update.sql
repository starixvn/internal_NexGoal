
CREATE PROCEDURE [dbo].[Period_Update] @PeriodId INT
	,@Name NVARCHAR(200)
	,@StartDate DATE = NULL
	,@EndDate DATE = NULL
	,@DisciplineStartDate DATE = NULL
	,@DisciplineEndDate DATE = NULL
	,@FocusStartDate DATE = NULL
	,@FocusEndDate DATE = NULL
	,@Description NVARCHAR(MAX) = NULL
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180727@Elton.Ton
Task: update period
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @TargetTypeId INT
		,@Today DATE = GETDATE()
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT @TargetTypeId = p.TargetTypeId
	FROM dbo.Period p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

	IF EXISTS (
			SELECT 1
			FROM dbo.Period AS p WITH (NOLOCK)
			WHERE p.[Name] = @Name
				AND p.TargetTypeId = @TargetTypeId
				AND p.PeriodId <> @PeriodId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Period ' + @Name + ' has already existed.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.TargetTypeId = 2
				AND @TargetTypeId = 2
				AND @StartDate <= p.EndDate
				AND @EndDate >= p.StartDate
				AND p.PeriodId <> @PeriodId
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
		,@logOldData NVARCHAR(MAX)
		,@xml XML

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
				,p.ModifiedBy AS '@mb'
				,p.ModifiedDate AS '@md'
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].[Period] WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,[Description] = @Description
		,[StartDate] = @StartDate
		,[EndDate] = @EndDate
		,[DisciplineStartDate] = @DisciplineStartDate
		,[DisciplineEndDate] = @DisciplineEndDate
		,[FocusStartDate] = @FocusStartDate
		,[FocusEndDate] = @FocusEndDate
		,[IsActive] = @IsActive
		,[ModifiedBy] = @ModifiedBy
		,[ModifiedDate] = GETDATE()
	WHERE PeriodId = @PeriodId

	UPDATE pa
	SET pa.StatusId = (CASE 
			WHEN @Today >= reviewer.StartDate
				AND (
					@Today <= reviewer.EndDate
					OR @Today <= period.EndDate
					)
				AND pa.StatusId <> @PAInProgress THEN @PAInProgress
			WHEN period.EndDate < @Today
				AND reviewer.EndDate < @Today
				AND pa.StatusId <> @PAClosed THEN @PAClosed
			ELSE pa.StatusId END
			)
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE period.PeriodId = @PeriodId
		AND pa.StatusId IN (@PAOpened, @PAInProgress, @PAClosed)

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
				,p.ModifiedBy AS '@mb'
				,p.ModifiedDate AS '@md'
			FROM dbo.Period p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_Update'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@Duration = @Duration
END
