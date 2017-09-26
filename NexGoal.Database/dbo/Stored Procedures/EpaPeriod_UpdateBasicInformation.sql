





/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateBasicInformation    Script Date: 7/21/2016 2:45:53 PM ******/
CREATE PROCEDURE [dbo].[EpaPeriod_UpdateBasicInformation] @PeriodId INT
	,@Name NVARCHAR(200) = NULL
	,@TargetTypeId INT
	,@StartDate DATE = NULL
	,@EndDate DATE = NULL
	,@AttendanceStartDate DATE = NULL
	,@AttendanceEndDate DATE = NULL
	,@PAFocusStartDate DATE = NULL
	,@IsActive BIT = 1
	,@Description NVARCHAR(MAX) = NULL
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Update basic information of evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #EmployeeIds (EmployeeId INT)

	DECLARE @Duration INT
		,@EmployeeId INT

	IF NOT EXISTS (
			SELECT 1
			FROM [dbo].[Period] WITH (NOLOCK)
			WHERE [PeriodId] = @PeriodId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' does not exists.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[Period] WITH (NOLOCK)
			WHERE [PeriodId] <> @PeriodId
				AND [Name] = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Period ''' + @Name + ''' has already existed.'

		RETURN
	END

	IF @TargetTypeId = 2 -- monthly
		AND EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE [PeriodId] <> @PeriodId
				AND TargetTypeId = 2
				AND (
					CAST(@StartDate AS DATE) BETWEEN CAST(StartDate AS DATE)
						AND CAST(EndDate AS DATE)
					OR CAST(@EndDate AS DATE) BETWEEN CAST(StartDate AS DATE)
						AND CAST(EndDate AS DATE)
					OR CAST(StartDate AS DATE) BETWEEN CAST(@StartDate AS DATE)
						AND CAST(@EndDate AS DATE)
					OR CAST(EndDate AS DATE) BETWEEN CAST(@StartDate AS DATE)
						AND CAST(@EndDate AS DATE)
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Start Date/End Date overlap on another period.'

		RETURN
	END

	DECLARE @Data NVARCHAR(MAX)
		,@OldData NVARCHAR(MAX)
		,@OldEndDate DATE

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @PeriodId
		,@ObjectType = 8

	SELECT @Duration = SUM(Duration) - 1
	FROM dbo.PAFocusProcessSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	BEGIN TRY
		BEGIN TRANSACTION

		SELECT @OldEndDate = EndDate
		FROM dbo.Period WITH (NOLOCK)
		WHERE PeriodId = @PeriodId

		UPDATE [dbo].[Period] WITH (ROWLOCK, UPDLOCK)
		SET [Name] = @Name
			,[TargetTypeId] = @TargetTypeId
			,[StartDate] = @StartDate
			,[EndDate] = @EndDate
			,[AttendanceStartDate] = @AttendanceStartDate
			,[AttendanceEndDate] = @AttendanceEndDate
			,[PAFocusStartDate] = @PAFocusStartDate
			,[PAFocusEndDate] = DATEADD(day, @Duration, @PAFocusStartDate)
			,[IsActive] = @IsActive
			,[Description] = @Description
			,[ModifiedBy] = @ModifiedBy
			,[ModifiedDate] = GETDATE()
		WHERE [PeriodId] = @PeriodId

		DELETE
		FROM dbo.PAFocusProcess
		WHERE PAFocusId IN (
				SELECT PAFocusId
				FROM dbo.PAFocus
				WHERE PeriodId = @PeriodId
				)

		INSERT INTO #EmployeeIds
		SELECT EmployeeId
		FROM dbo.PAFocus WITH (NOLOCK)
		WHERE PeriodId = @PeriodId

		WHILE (
				EXISTS (
					SELECT 1
					FROM #EmployeeIds
					)
				)
		BEGIN
			SELECT TOP 1 @EmployeeId = EmployeeId
			FROM #EmployeeIds

			EXEC dbo.[PAFocusProcess_Generate] @periodId
				,@EmployeeId

			DELETE
			FROM #EmployeeIds
			WHERE EmployeeId = @EmployeeId
		END

		IF (
				ISNULL(@OldEndDate, '') <> ''
				AND ISNULL(@EndDate, '') <> ''
				AND @OldEndDate <> @EndDate
				)
			OR (
				ISNULL(@OldEndDate, '') = ''
				AND ISNULL(@EndDate, '') <> ''
				)
		BEGIN
			UPDATE epa
			SET epa.[Status] = 0 -- open
				,epa.ModifiedBy = @ModifiedBy
				,epa.ModifiedDate = GETDATE()
			FROM dbo.EPA AS epa
				INNER JOIN (
				SELECT DISTINCT EPAId
					,MAX(EndDate) AS EndDate
				FROM dbo.EPAProcess
				GROUP BY EPAId
				) AS pc ON epa.EPAId = pc.EPAId
				INNER JOIN dbo.Period AS period ON epa.PeriodId = period.PeriodId
			WHERE period.PeriodId = @PeriodId
				AND epa.[Status] = 1
				AND CAST(period.EndDate AS DATE) > CAST(pc.EndDate AS DATE)
		END

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog] @ObjectId = @PeriodId
			,@ObjectType = 8

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaPeriod_UpdateBasicInformation'
			,@ObjectId = @PeriodId
			,@ObjectTypeId = 8
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update Period unsuccessfully.'
	END CATCH

	DROP TABLE #EmployeeIds;
END
