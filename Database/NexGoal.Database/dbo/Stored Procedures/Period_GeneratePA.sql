
CREATE PROCEDURE [dbo].[Period_GeneratePA] @PeriodId INT
	,@PARecordIds VARCHAR(MAX) = NULL
	,@CreatedBy INT
	,@Override BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180827@Elton.ton
Task: generate pa
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @periodStartDate DATE
		,@periodEndDate DATE
		,@workingFromDate DATETIME
		,@workingToDate DATETIME
		,@dateFormat VARCHAR(11) = 'dd MMM yyyy'
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		-- Performance Status
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		-- record type
		,@CheckinTypeId TINYINT = 1
		,@PATypeId TINYINT = 2
	DECLARE @today DATE = GETDATE()
		,@createdDate DATETIME = GETDATE()
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tblPARecord (
		PARecordId INT PRIMARY KEY
		,EmployeeId INT
		)

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT PRIMARY KEY
		,EmployeeId INT
		,EvaluatorId INT
		,ProcessId TINYINT
		)

	INSERT INTO #tblPARecord (
		PARecordId
		,EmployeeId
		)
	SELECT pa.PARecordId
		,pa.EmployeeId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.func_SplitString(@PARecordIds, ',') AS func ON ISNUMERIC(func.val) = 1
		AND func.val = pa.PARecordId
	WHERE pa.PeriodId = @PeriodId

	SELECT @periodStartDate = StartDate
		,@periodEndDate = EndDate
		,@workingFromDate = DisciplineStartDate
		,@workingToDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SET @xml = (
			SELECT process.PARecordId AS '@pari'
				,process.ProcessId AS '@pi'
				,process.EvaluatorId AS '@ei'
				,pr.PerformanceRecordId AS '@pri'
				,pr.EmployeeId AS '@pei'
				,pr.GradeId AS '@pgi'
				,pr.Comment AS '@pc'
				,pr.CreatedDate AS '@pcd'
				,pr.ModifiedDate AS '@pmd'
			FROM dbo.PAProcess AS process WITH (NOLOCK)
				INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
				LEFT JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON process.PerformanceRecordId = pr.PerformanceRecordId
			WHERE process.ProcessId >= @SelfProcessId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE record
	FROM dbo.PerformanceRecord AS record
	WHERE record.TypeId = @PATypeId
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS process WITH (NOLOCK)
				INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
			WHERE process.PerformanceRecordId = record.PerformanceRecordId
				AND process.ProcessId = @PMProcessId
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.PMSetting AS pm WITH (NOLOCK)
					WHERE process.EvaluatorId = pm.ProjectManagerId
						AND pm.MemberId = pa.EmployeeId
					)
			)

	DELETE process
	FROM dbo.PAProcess AS process
		INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
	WHERE process.ProcessId = @PMProcessId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PMSetting AS pm WITH (NOLOCK)
			WHERE process.EvaluatorId = pm.ProjectManagerId
				AND pm.MemberId = pa.EmployeeId
			)

	-- capture for PM
	INSERT INTO dbo.PerformanceRecord (
		EmployeeId
		,CreatorId
		,GradeId
		,Comment
		,CreatedDate
		,TypeId
		,RoleId
		)
	OUTPUT INSERTED.PerformanceRecordId
		,INSERTED.EmployeeId
		,INSERTED.CreatorId
		,INSERTED.RoleId
	INTO #tblPerformanceRecord(PerformanceRecordId, EmployeeId, EvaluatorId, ProcessId)
	SELECT pa.EmployeeId
		,pm.ProjectManagerId
		,lg.GradeId
		,STUFF((
				SELECT CHAR(10) + CHAR(10) + FORMAT(prcmt.CreatedDate, @dateFormat) + CHAR(10) + prcmt.Comment
				FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
				WHERE prcmt.CreatorId = pm.ProjectManagerId
					AND prcmt.EmployeeId = pa.EmployeeId
					AND prcmt.TypeId = @CheckinTypeId
					AND prcmt.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND prcmt.RoleId = @PMProcessId
				ORDER BY prcmt.CreatedDate DESC
				FOR XML PATH
					,TYPE
				).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'')
		,GETDATE()
		,@PATypeId
		,@PMProcessId
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PMSetting AS pm WITH (NOLOCK) ON pa.EmployeeId = pm.MemberId
		OUTER APPLY (
		SELECT TOP 1 record.GradeId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = @CheckinTypeId
			AND record.CreatorId = pm.ProjectManagerId
			AND record.EmployeeId = pa.EmployeeId
			AND record.CreatedDate BETWEEN @workingFromDate
				AND @workingToDate
			AND record.RoleId = @PMProcessId
			AND ISNULL(record.GradeId, '') <> ''
		ORDER BY record.CreatedDate DESC
		) AS lg
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.CreatorId = pm.ProjectManagerId
				AND record.EmployeeId = pa.EmployeeId
				AND record.CreatedDate BETWEEN @workingFromDate
					AND @workingToDate
				AND record.RoleId = @PMProcessId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS process WITH (NOLOCK)
			WHERE pa.PARecordId = process.PARecordId
				AND process.EvaluatorId = pm.ProjectManagerId
				AND (
					process.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
					OR (
						process.ProcessId = @PMProcessId
						AND process.PerformanceRecordId IS NOT NULL
						)
					)
			)

	-- Capture for Self, Reviewer
	INSERT INTO dbo.PerformanceRecord (
		EmployeeId
		,CreatorId
		,GradeId
		,Comment
		,CreatedDate
		,TypeId
		,RoleId
		)
	OUTPUT INSERTED.PerformanceRecordId
		,INSERTED.EmployeeId
		,INSERTED.CreatorId
		,INSERTED.RoleId
	INTO #tblPerformanceRecord(PerformanceRecordId, EmployeeId, EvaluatorId, ProcessId)
	SELECT pa.EmployeeId
		,paprocess.EvaluatorId
		,lg.GradeId
		,STUFF((
				SELECT CHAR(10) + CHAR(10) + FORMAT(prcmt.CreatedDate, @dateFormat) + CHAR(10) + prcmt.Comment
				FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
				WHERE prcmt.CreatorId = paprocess.EvaluatorId
					AND prcmt.EmployeeId = pa.EmployeeId
					AND prcmt.TypeId = @CheckinTypeId
					AND prcmt.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND prcmt.RoleId = paprocess.ProcessId
				ORDER BY prcmt.CreatedDate DESC
				FOR XML PATH
					,TYPE
				).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'')
		,GETDATE()
		,@PATypeId
		,paprocess.ProcessId
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PAProcess AS paprocess WITH (NOLOCK) ON pa.PARecordId = paprocess.PARecordId
		OUTER APPLY (
		SELECT TOP 1 record.GradeId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = @CheckinTypeId
			AND record.CreatorId = paprocess.EvaluatorId
			AND record.EmployeeId = pa.EmployeeId
			AND record.CreatedDate BETWEEN @workingFromDate
				AND @workingToDate
			AND record.RoleId = paprocess.ProcessId
			AND ISNULL(record.GradeId, '') <> ''
		ORDER BY record.CreatedDate DESC
		) AS lg
	WHERE paprocess.ProcessId IN (@SelfProcessId, @ReviewerProcessId)
		AND paprocess.PerformanceRecordId IS NULL
		AND EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.CreatorId = paprocess.EvaluatorId
				AND record.EmployeeId = pa.EmployeeId
				AND record.CreatedDate BETWEEN @workingFromDate
					AND @workingToDate
				AND record.RoleId = paprocess.ProcessId
			)

	-- Capture for Evaluator
	INSERT INTO dbo.PerformanceRecord (
		EmployeeId
		,CreatorId
		,GradeId
		,Comment
		,CreatedDate
		,TypeId
		,RoleId
		)
	OUTPUT INSERTED.PerformanceRecordId
		,INSERTED.EmployeeId
		,INSERTED.CreatorId
		,INSERTED.RoleId
	INTO #tblPerformanceRecord(PerformanceRecordId, EmployeeId, EvaluatorId, ProcessId)
	SELECT pa.EmployeeId
		,paprocess.EvaluatorId
		,lg.GradeId
		,STUFF((
				SELECT CHAR(10) + CHAR(10) + FORMAT(prcmt.CreatedDate, @dateFormat) + CHAR(10) + prcmt.Comment
				FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
				WHERE prcmt.CreatorId = paprocess.EvaluatorId
					AND prcmt.EmployeeId = pa.EmployeeId
					AND prcmt.TypeId = @CheckinTypeId
					AND prcmt.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND prcmt.RoleId = paprocess.ProcessId
				ORDER BY prcmt.CreatedDate DESC
				FOR XML PATH
					,TYPE
				).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'')
		,GETDATE()
		,@PATypeId
		,paprocess.ProcessId
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PAProcess AS paprocess WITH (NOLOCK) ON pa.PARecordId = paprocess.PARecordId
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
	WHERE paprocess.ProcessId = @EvaluatorProcessId
		AND paprocess.PerformanceRecordId IS NULL
		AND EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.EmployeeId = pa.EmployeeId
				AND record.RoleId = @EvaluatorProcessId
				AND (
					(
						record.CreatorId = paprocess.EvaluatorId
						AND record.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						)
					OR (
						record.CreatedDate < @workingFromDate
						AND ISNULL(record.GradeId, '') <> ''
						)
					)
			)

	-- Insert new PM Process
	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,PerformanceRecordId
		)
	SELECT pa.PARecordId
		,@PMProcessId
		,record.EvaluatorId
		,record.PerformanceRecordId
	FROM #tblPerformanceRecord AS record
		INNER JOIN #tblPARecord AS pa ON record.EmployeeId = pa.EmployeeId
	WHERE record.ProcessId = @PMProcessId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS process WITH (NOLOCK)
			WHERE pa.PARecordId = process.PARecordId
				AND process.ProcessId = @PMProcessId
				AND process.EvaluatorId = record.EvaluatorId
			)

	UPDATE paprocess
	SET paprocess.PerformanceRecordId = record.PerformanceRecordId
	FROM dbo.PAProcess AS paprocess WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPARecord AS pa ON paprocess.PARecordId = pa.PARecordId
		INNER JOIN #tblPerformanceRecord AS record ON pa.EmployeeId = record.EmployeeId
		AND paprocess.EvaluatorId = record.EvaluatorId
		AND paprocess.ProcessId = record.ProcessId

	UPDATE pa
	SET StatusId = (CASE 
			WHEN @today >= reviewer.StartDate
				AND (
					@today <= reviewer.EndDate
					OR @today <= @periodEndDate
					) THEN @PAInProgress
			WHEN @periodEndDate < @today
				AND reviewer.EndDate < @today THEN @PAClosed
			WHEN @today < reviewer.StartDate THEN @PAOpened
			ELSE pa.StatusId END
			)
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE EXISTS (
			SELECT 1
			FROM #tblPARecord AS tbl
			WHERE tbl.PARecordId = pa.PARecordId
			)

	-- update if @overide = 1
	IF @Override = 1
	BEGIN
		UPDATE record
		SET record.RoleId = paprocess.ProcessId
		FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.PAProcess AS paprocess WITH (NOLOCK) ON record.PerformanceRecordId = paprocess.PerformanceRecordId
		WHERE record.RoleId IS NULL
			AND EXISTS (
				SELECT 1
				FROM #tblPARecord AS pa
				WHERE paprocess.PARecordId = pa.PARecordId
				)

		-- update for Self, PM, Reviewer
		DELETE record
		FROM dbo.PerformanceRecord AS record
			INNER JOIN dbo.PAProcess AS paprocess WITH (NOLOCK) ON record.PerformanceRecordId = paprocess.PerformanceRecordId
			AND paprocess.ProcessId IN (@SelfProcessId, @PMProcessId, @ReviewerProcessId)
			INNER JOIN #tblPARecord AS pa ON paprocess.PARecordId = pa.PARecordId
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.PerformanceRecord AS record1 WITH (NOLOCK)
				WHERE record1.TypeId = @CheckinTypeId
					AND record1.CreatorId = paprocess.EvaluatorId
					AND record1.EmployeeId = pa.EmployeeId
					AND record1.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND record1.RoleId = paprocess.ProcessId
				)

		UPDATE paprocess
		SET paprocess.PerformanceRecordId = NULL
		FROM dbo.PAProcess AS paprocess WITH (ROWLOCK, UPDLOCK)
		WHERE EXISTS (
				SELECT 1
				FROM #tblPARecord AS pa
				WHERE paprocess.PARecordId = pa.PARecordId
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				WHERE paprocess.PerformanceRecordId = record.PerformanceRecordId
				)

		UPDATE pr
		SET pr.GradeId = lg.GradeId
			,pr.Comment = STUFF((
					SELECT CHAR(10) + CHAR(10) + FORMAT(prcmt.CreatedDate, @dateFormat) + CHAR(10) + prcmt.Comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeId = pa.EmployeeId
						AND prcmt.CreatorId = paprocess.EvaluatorId
						AND prcmt.TypeId = @CheckinTypeId
						AND prcmt.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						AND prcmt.RoleId = paprocess.ProcessId
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH
						,TYPE
					).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'')
		FROM dbo.PerformanceRecord AS pr WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.PAProcess AS paprocess WITH (NOLOCK) ON pr.PerformanceRecordId = paprocess.PerformanceRecordId
			AND paprocess.ProcessId IN (@SelfProcessId, @PMProcessId, @ReviewerProcessId)
			AND pr.RoleId = paprocess.ProcessId
			INNER JOIN #tblPARecord AS pa ON pa.PARecordId = paprocess.PARecordId
			OUTER APPLY (
			SELECT TOP 1 record.GradeId
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.CreatorId = paprocess.EvaluatorId
				AND record.EmployeeId = pa.EmployeeId
				AND record.CreatedDate BETWEEN @workingFromDate
					AND @workingToDate
				AND record.RoleId = paprocess.ProcessId
				AND ISNULL(record.GradeId, '') <> ''
			ORDER BY record.CreatedDate DESC
			) AS lg
		WHERE EXISTS (
				SELECT 1
				FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				WHERE record.TypeId = @CheckinTypeId
					AND record.CreatorId = paprocess.EvaluatorId
					AND record.EmployeeId = pa.EmployeeId
					AND record.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND record.RoleId = paprocess.ProcessId
				)

		-- update for Evaluator
		UPDATE pr
		SET pr.GradeId = lg.GradeId
			,pr.Comment = STUFF((
					SELECT CHAR(10) + CHAR(10) + FORMAT(prcmt.CreatedDate, @dateFormat) + CHAR(10) + prcmt.Comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeId = pa.EmployeeId
						AND prcmt.CreatorId = paprocess.EvaluatorId
						AND prcmt.TypeId = @CheckinTypeId
						AND prcmt.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						AND prcmt.RoleId = paprocess.ProcessId
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH
						,TYPE
					).value(N'.[1]', N'NVARCHAR(MAX)'), 1, 2, N'')
		FROM dbo.PerformanceRecord AS pr WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.PAProcess AS paprocess WITH (NOLOCK) ON pr.PerformanceRecordId = paprocess.PerformanceRecordId
			AND paprocess.ProcessId = @EvaluatorProcessId
			INNER JOIN #tblPARecord AS pa ON pa.PARecordId = paprocess.PARecordId
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
					AND record.RoleId = paprocess.ProcessId
					AND (
						(
							record.CreatorId = paprocess.EvaluatorId
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

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
	SET @xml = (
			SELECT paprocess.PARecordId AS '@pari'
				,paprocess.ProcessId AS '@pi'
				,paprocess.EvaluatorId AS '@ei'
				,pr.PerformanceRecordId AS '@pri'
				,pr.EmployeeId AS '@pei'
				,pr.GradeId AS '@pgi'
				,pr.Comment AS '@pc'
				,pr.CreatedDate AS '@pcd'
				,pr.ModifiedDate AS '@pmd'
			FROM dbo.PAProcess AS paprocess WITH (NOLOCK)
				INNER JOIN #tblPARecord AS pa ON paprocess.PARecordId = pa.PARecordId
				LEFT JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON paprocess.PerformanceRecordId = pr.PerformanceRecordId
			WHERE paprocess.ProcessId >= @SelfProcessId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'Period_GeneratePA'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@Duration = @Duration

	DROP TABLE #tblPARecord

	DROP TABLE #tblPerformanceRecord
END
