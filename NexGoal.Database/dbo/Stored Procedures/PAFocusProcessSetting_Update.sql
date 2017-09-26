


/****** Object:  Stored Procedure dbo.EpaPeriod_Insert    Script Date: 7/21/2016 2:45:53 PM ******/
CREATE PROCEDURE [dbo].[PAFocusProcessSetting_Update] @PeriodId INT
	,@PAFocusProcessSettings NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160830@Razor.nguyen
Task: Focus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
<ROOT>
    <PAFocusProcessSetting ProcessId='{0}' Duration='{1}'></PAFocusProcessSetting>
</ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DocHandler INT
		,@EmployeeId INT
		,@Duration INT

	IF OBJECT_ID('tempdb..#FocusProcessSetting') IS NOT NULL
	BEGIN
		DROP TABLE #FocusProcessSetting;
	END;

	IF OBJECT_ID('tempdb..#EmployeeIds') IS NOT NULL
	BEGIN
		DROP TABLE #EmployeeIds;
	END;

	CREATE TABLE #EmployeeIds (EmployeeId INT)

	CREATE TABLE #FocusProcessSetting (
		ProcessId INT
		,Duration INT
		);

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@PAFocusProcessSettings

		INSERT INTO #FocusProcessSetting
		SELECT ProcessId
			,Duration
		FROM OPENXML(@DocHandler, '/ROOT/PAFocusProcessSetting', 1) WITH (ProcessId INT, Duration INT) AS Data;

		UPDATE t1
		SET t1.Duration = t2.Duration
		FROM dbo.PAFocusProcessSetting AS t1
			INNER JOIN #FocusProcessSetting AS t2 ON t2.ProcessId = t1.ProcessId
		WHERE t1.PeriodId = @PeriodId

		SELECT @Duration = SUM(Duration)
		FROM dbo.PAFocusProcessSetting WITH (NOLOCK)
		WHERE PeriodId = @PeriodId

		UPDATE dbo.Period
		SET PAFocusEndDate = Dateadd(day, @Duration - 1, PAFocusStartDate)
		WHERE PeriodId = @periodId

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

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 1
			,@ErrMsg = N'Evaluate unsuccessfully.';
	END CATCH

	DROP TABLE #FocusProcessSetting
END