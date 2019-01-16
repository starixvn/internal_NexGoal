
CREATE PROCEDURE [dbo].[Auto_Period_Insert] @ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181031@Tim.Trinh
Task: auto create period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATE = GETDATE()
		,@Month TINYINT = DATEPART(MONTH, GETDATE())
		,@Year SMALLINT = DATEPART(YEAR, GETDATE())
		,@Name NVARCHAR(200)
		,@SemiTargetTypeId TINYINT = 2

	SET @Name = '1H ' + CAST(@Year AS VARCHAR(5)) + ' - Auto'

	IF @Month BETWEEN 6
			AND 11
	BEGIN
		SET @Name = '2H ' + CAST(@Year AS VARCHAR(5)) + ' - Auto'
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE DisciplineEndDate > @Today
				AND TargetTypeId = @SemiTargetTypeId
			)
	BEGIN
		DECLARE @StartDate DATE
			,@EndDate DATE
			,@DisciplineStartDate DATE
			,@DisciplineEndDate DATE
			,@FocusStartDate DATE
			,@FocusEndDate DATE

		SELECT @DisciplineEndDate = MAX(DisciplineEndDate)
		FROM dbo.Period WITH (NOLOCK)
		WHERE TargetTypeId = @SemiTargetTypeId
		
		SET @DisciplineEndDate = ISNULL(@DisciplineEndDate, @Today)
		SET @StartDate = DATEADD(DAY, 1 - DAY(DATEADD(MONTH, 6, @DisciplineEndDate)), DATEADD(MONTH, 6, @DisciplineEndDate))
		SET @EndDate = DATEADD(DAY, - 1, DATEADD(MONTH, 1, @StartDate))
		SET @DisciplineStartDate = DATEADD(DAY, 1, @DisciplineEndDate)
		SET @DisciplineEndDate = @EndDate
		SET @FocusStartDate = DATEADD(DAY, - 7, @EndDate)
		SET @FocusEndDate = @EndDate

		INSERT INTO dbo.Period (
			[Name]
			,TargetTypeId
			,StartDate
			,EndDate
			,DisciplineStartDate
			,DisciplineEndDate
			,FocusStartDate
			,FocusEndDate
			,IsActive
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@Name
			,@SemiTargetTypeId
			,@StartDate
			,@EndDate
			,@DisciplineStartDate
			,@DisciplineEndDate
			,@FocusStartDate
			,@FocusEndDate
			,1
			,- 1
			,GETDATE()
			)

		DECLARE @PeriodId INT = SCOPE_IDENTITY()

		INSERT INTO dbo.PeriodProcessSetting (
			PeriodId
			,ProcessId
			,Duration
			,NoticeAtBegin
			,RemindDay
			,FocusDay
			)
		SELECT @PeriodId
			,ProcessId
			,Duration
			,NoticeAtBegin
			,RemindDay
			,FocusDay
		FROM dbo.PeriodDefaultProcessSetting WITH (NOLOCK)
		WHERE TargetTypeId = @SemiTargetTypeId
			AND ProcessId = 5 -- reviewer
	END
END