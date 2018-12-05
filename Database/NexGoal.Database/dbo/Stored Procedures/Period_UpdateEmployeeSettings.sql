
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateProcessSetting    Script Date: 7/21/2016 2:45:54 PM ******/
CREATE PROCEDURE [dbo].[Period_UpdateEmployeeSettings] @PeriodId INT
	,@Settings VARCHAR(MAX)
	,@PARecordIds VARCHAR(MAX)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180904@Elton.Ton
Task: Update period employee setting.
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)
		,@xml XML
		,@docHandler INT
	--
	DECLARE @Today DATE = GETDATE()
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@HRProcessId TINYINT = 1
		,@ReviewerProcessId TINYINT = 5

	EXEC sp_xml_preparedocument @docHandler OUTPUT
		,@Settings

	DECLARE @employeeSetting TABLE (
		ProcessId TINYINT
		,EvaluatorId INT
		,StartDate DATE
		,EndDate DATE
		)
	DECLARE @paRecordIdTable TABLE (Id INT)

	INSERT INTO @paRecordIdTable (Id)
	SELECT val
	FROM dbo.func_SplitString(@PARecordIds, ',')

	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pap.StartDate AS '@sd'
				,pap.EndDate AS '@ed'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM @paRecordIdTable AS pari
					WHERE pap.PARecordId = pari.Id
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	INSERT INTO @employeeSetting (
		ProcessId
		,EvaluatorId
		,StartDate
		,EndDate
		)
	SELECT ProcessId
		,NULLIF(EvaluatorId, '')
		,NULLIF(StartDate, '')
		,NULLIF(EndDate, '')
	FROM OPENXML(@docHandler, '/Root/Row', 1) WITH (ProcessId TINYINT, EvaluatorId INT, StartDate DATE, EndDate DATE)

	IF EXISTS (
			SELECT 1
			FROM @employeeSetting
			WHERE ProcessId = @HRProcessId
				AND EvaluatorId IS NOT NULL
			)
	BEGIN
		--Insert new evaluators		
		INSERT INTO dbo.PAProcess (
			PARecordId
			,ProcessId
			,EvaluatorId
			,[Priority]
			,StartDate
			,EndDate
			)
		SELECT pari.Id
			,es.ProcessId
			,es.EvaluatorId
			,0
			,ISNULL(es.StartDate, (
					SELECT TOP 1 pap.StartDate
					FROM dbo.PAProcess AS pap WITH (NOLOCK)
					WHERE pap.PARecordId = pari.Id
						AND pap.ProcessId = @HRProcessId
					))
			,ISNULL(es.EndDate, (
					SELECT TOP 1 pap.EndDate
					FROM dbo.PAProcess AS pap WITH (NOLOCK)
					WHERE pap.PARecordId = pari.Id
						AND pap.ProcessId = @HRProcessId
					))
		FROM @paRecordIdTable AS pari
			,@employeeSetting AS es
		WHERE es.ProcessId = @HRProcessId
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.PAProcess AS pap WITH (NOLOCK)
				WHERE pap.PARecordId = pari.Id
					AND pap.EvaluatorId = es.EvaluatorId
					AND pap.ProcessId = @HRProcessId
				)

		--Update Evaluators
		UPDATE pap
		SET StartDate = ISNULL(es.StartDate, pap.StartDate)
			,EndDate = ISNULL(es.EndDate, pap.EndDate)
		FROM dbo.PAProcess AS pap WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @employeeSetting AS es ON es.ProcessId = @HRProcessId
			AND pap.EvaluatorId = es.EvaluatorId
			INNER JOIN @paRecordIdTable AS pari ON pap.PARecordId = pari.Id
		WHERE pap.ProcessId = @HRProcessId

		--deleted evaluators
		DELETE pap
		FROM dbo.PAProcess AS pap
			INNER JOIN @paRecordIdTable AS pari ON pap.PARecordId = pari.Id
		WHERE pap.ProcessId = 1
			AND NOT EXISTS (
				SELECT 1
				FROM @employeeSetting AS es
				WHERE es.ProcessId = @HRProcessId
					AND es.EvaluatorId = pap.EvaluatorId
				)
	END

	SELECT pap.PARecordId
		,pap.ProcessId
	INTO #changedProcessTbl
	FROM dbo.PAProcess AS pap WITH (NOLOCK)
		INNER JOIN @paRecordIdTable AS pari ON pap.PARecordId = pari.Id
		INNER JOIN @employeeSetting AS es ON es.ProcessId = pap.ProcessId
	WHERE pap.EvaluatorId <> es.EvaluatorId
		AND pap.PerformanceRecordId IS NOT NULL
		AND pap.ProcessId > @HRProcessId
		AND es.EvaluatorId IS NOT NULL

	UPDATE pap
	SET EvaluatorId = ISNULL(es.EvaluatorId, pap.EvaluatorId)
		,StartDate = ISNULL(es.StartDate, pap.StartDate)
		,EndDate = ISNULL(es.EndDate, pap.EndDate)
	FROM dbo.PAProcess AS pap WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @paRecordIdTable AS pari ON pap.PARecordId = pari.Id
		INNER JOIN @employeeSetting AS es ON es.ProcessId = pap.ProcessId
	WHERE pap.ProcessId > @HRProcessId

	-- update PA status
	UPDATE pa
	SET pa.StatusId = (CASE 
			WHEN @Today BETWEEN hr.StartDate
					AND reviewer.EndDate
				AND pa.StatusId <> @PAInProgress THEN @PAInProgress
			WHEN period.EndDate < @Today
				AND reviewer.EndDate < @Today
				AND pa.StatusId <> @PAClosed THEN @PAClosed
			ELSE pa.StatusId END
			)
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @paRecordIdTable AS paid ON pa.PARecordId = paid.Id
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		INNER JOIN (
		SELECT DISTINCT pa1.PARecordId
			,StartDate
			,EndDate
		FROM dbo.PARecord AS pa1 WITH (NOLOCK)
			INNER JOIN @paRecordIdTable AS paid1 ON pa1.PARecordId = paid1.Id
			INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa1.PARecordId = process.PARecordId
		WHERE pa1.StatusId IN (@PAOpened, @PAInProgress, @PAClosed)
			AND ProcessId = @HRProcessId
		) AS hr ON pa.PARecordId = hr.PARecordId
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE pa.StatusId IN (@PAOpened, @PAInProgress, @PAClosed)

	IF EXISTS (
			SELECT 1
			FROM #changedProcessTbl
			)
	BEGIN
		DECLARE @disciplineStartDate DATE
			,@disciplineEndDate DATE
		DECLARE @PRSubmitted TINYINT = 3
			,@PRClosed TINYINT = 4

		SELECT @disciplineStartDate = p.DisciplineStartDate
			,@disciplineEndDate = p.DisciplineEndDate
		FROM dbo.Period AS p WITH (NOLOCK)
		WHERE p.PeriodId = @PeriodId

		UPDATE pr
		SET pr.CreatorId = pap.EvaluatorId
			,pr.GradeId = lg.GradeId
			,pr.Comment = STUFF((
					SELECT CHAR(10) + prcmt.comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeIds = pr.EmployeeIds
						AND prcmt.CreatorId = lg.CreatorId
						AND prcmt.TypeId = 1
						AND prcmt.CreatedDate BETWEEN @disciplineStartDate
							AND @disciplineEndDate
						AND prcmt.StatusId IN (@PRSubmitted, @PRClosed)
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, '')
			,pr.ModifiedDate = GETDATE()
		FROM dbo.PerformanceRecord AS pr WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.PAProcess AS pap WITH (NOLOCK) ON pap.PerformanceRecordId = pr.PerformanceRecordId
			INNER JOIN #changedProcessTbl AS cpt ON cpt.PARecordId = pap.PARecordId
			AND cpt.ProcessId = pap.ProcessId
			OUTER APPLY (
			SELECT TOP 1 pr.CreatorId
				,pr.GradeId
			FROM dbo.PerformanceRecord AS pr1 WITH (NOLOCK)
			WHERE pr1.EmployeeIds = pr.EmployeeIds
				AND pr1.CreatorId = pap.EvaluatorId
				AND pr1.TypeId = 1
				AND pr1.CreatedDate <= @disciplineEndDate
				AND pr1.StatusId IN (@PRSubmitted, @PRClosed)
			ORDER BY pr1.CreatedDate DESC
			) lg
	END

	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pap.StartDate AS '@sd'
				,pap.EndDate AS '@ed'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM @paRecordIdTable AS pari
					WHERE pap.PARecordId = pari.Id
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_UpdateEmployeeSettings'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@OldData = @logOldData
		,@Duration = @Duration

	DROP TABLE #changedProcessTbl
END
