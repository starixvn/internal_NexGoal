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
DB: bodb_NexGoal
Original:

Revisions:
	20200221@Tim.Trinh: get related performance record
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

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
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	CREATE TABLE #changedProcessTbl (
		PARecordId INT
		,ProcessId TINYINT
		)

	EXEC sp_xml_preparedocument @docHandler OUTPUT
		,@Settings

	DECLARE @tblEmployeeSetting TABLE (
		ProcessId TINYINT
		,EvaluatorId INT
		,StartDate DATE
		,EndDate DATE
		)
	DECLARE @tblPARecord TABLE (
		PARecordId INT
		,EmployeeId INT
		)

	INSERT INTO @tblPARecord (
		PARecordId
		,EmployeeId
		)
	SELECT pa.PARecordId
		,pa.EmployeeId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.func_SplitString(@PARecordIds, ',') AS func ON ISNUMERIC(func.val) = 1
		AND func.val = pa.PARecordId

	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pap.StartDate AS '@sd'
				,pap.EndDate AS '@ed'
				,pap.GradeId AS '@gid'
				,pap.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM @tblPARecord AS pari
					WHERE pap.PARecordId = pari.PARecordId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	INSERT INTO @tblEmployeeSetting (
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

	INSERT INTO #changedProcessTbl (
		PARecordId
		,ProcessId
		)
	SELECT pap.PARecordId
		,pap.ProcessId
	FROM dbo.PAProcess AS pap WITH (NOLOCK)
		INNER JOIN @tblPARecord AS pari ON pap.PARecordId = pari.PARecordId
		INNER JOIN @tblEmployeeSetting AS es ON es.ProcessId = pap.ProcessId
		AND es.EvaluatorId IS NOT NULL
	WHERE pap.EvaluatorId <> es.EvaluatorId
		AND pap.ProcessId >= @EvaluatorProcessId

	UPDATE pap
	SET EvaluatorId = ISNULL(es.EvaluatorId, pap.EvaluatorId)
		,StartDate = ISNULL(es.StartDate, pap.StartDate)
		,EndDate = ISNULL(es.EndDate, pap.EndDate)
	FROM dbo.PAProcess AS pap WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblPARecord AS pari ON pap.PARecordId = pari.PARecordId
		INNER JOIN @tblEmployeeSetting AS es ON es.ProcessId = pap.ProcessId
	WHERE pap.ProcessId >= @EvaluatorProcessId

	-- update PA status
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
			WHEN @Today < reviewer.StartDate THEN @PAOpened
			ELSE pa.StatusId END
			)
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblPARecord AS paid ON pa.PARecordId = paid.PARecordId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE pa.StatusId IN (@PAOpened, @PAInProgress, @PAClosed)

	IF EXISTS (
			SELECT 1
			FROM #changedProcessTbl
			)
	BEGIN
		DECLARE @workingFromDate DATE
			,@workingToDate DATE
			,@CheckinTypeId TINYINT = 1

		SELECT @workingFromDate = p.DisciplineStartDate
			,@workingToDate = p.DisciplineEndDate
		FROM dbo.Period AS p WITH (NOLOCK)
		WHERE p.PeriodId = @PeriodId

		-- update for Reviewer
		UPDATE process
		SET process.GradeId = lg.GradeId
			,process.RelatedPerformanceRecordIds = STUFF((
					SELECT ',' + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.CreatorId = process.EvaluatorId
						AND prcmt.EmployeeId = pa.EmployeeId
						AND prcmt.TypeId = @CheckinTypeId
						AND prcmt.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						AND prcmt.RoleId = process.ProcessId
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, N'')
		FROM dbo.PAProcess AS process WITH (NOLOCK)
			INNER JOIN @tblPARecord AS pa ON pa.PARecordId = process.PARecordId
			AND process.ProcessId = @ReviewerProcessId
			OUTER APPLY (
			SELECT TOP 1 record.GradeId
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.CreatorId = process.EvaluatorId
				AND record.EmployeeId = pa.EmployeeId
				AND record.CreatedDate BETWEEN @workingFromDate
					AND @workingToDate
				AND record.RoleId = process.ProcessId
				AND ISNULL(record.GradeId, '') <> ''
			ORDER BY record.CreatedDate DESC
			) AS lg
		WHERE EXISTS (
				SELECT 1
				FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				WHERE record.TypeId = @CheckinTypeId
					AND record.CreatorId = process.EvaluatorId
					AND record.EmployeeId = pa.EmployeeId
					AND record.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND record.RoleId = process.ProcessId
				)

		UPDATE process
		SET process.GradeId = lg.GradeId
			,process.RelatedPerformanceRecordIds = STUFF((
					SELECT ',' + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.CreatorId = process.EvaluatorId
						AND prcmt.EmployeeId = pa.EmployeeId
						AND prcmt.TypeId = @CheckinTypeId
						AND prcmt.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						AND prcmt.RoleId = process.ProcessId
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, N'')
		FROM dbo.PAProcess AS process WITH (NOLOCK)
			INNER JOIN @tblPARecord AS pa ON pa.PARecordId = process.PARecordId
			AND process.ProcessId = @EvaluatorProcessId
			OUTER APPLY (
			SELECT TOP 1 record.GradeId
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.EmployeeId = pa.EmployeeId
				AND record.RoleId = @EvaluatorProcessId
				AND record.CreatedDate <= @workingToDate
				AND ISNULL(record.GradeId, '') <> ''
			ORDER BY record.CreatedDate DESC
			) AS lg
		WHERE EXISTS (
				SELECT 1
				FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				WHERE record.TypeId = @CheckinTypeId
					AND record.EmployeeId = pa.EmployeeId
					AND record.RoleId = @EvaluatorProcessId
					AND (
						(
							record.CreatorId = process.EvaluatorId
							AND record.CreatedDate BETWEEN @workingFromDate
								AND @workingToDate
							)
						OR (
							record.CreatedDate < @workingFromDate
							AND ISNULL(record.GradeId, '') <> ''
							)
						)
				)
	END

	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pap.StartDate AS '@sd'
				,pap.EndDate AS '@ed'
				,pap.GradeId AS '@gid'
				,pap.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM @tblPARecord AS pari
					WHERE pap.PARecordId = pari.PARecordId
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
