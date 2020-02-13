
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
	20200212@Tim.Trinh: get related performance record
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @periodStartDate DATE
		,@periodEndDate DATE
		,@workingFromDate DATETIME
		,@workingToDate DATETIME
		,@separator NVARCHAR(42) = ','
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		-- Performance Status
		,@NotGenerated TINYINT = 1
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
		,StatusId TINYINT
		)

	CREATE TABLE #tblPAProcess (
		PARecordId INT
		,EvaluatorId INT
		,ProcessId TINYINT
		,GradeId VARCHAR(MAX)
		,RelatedPerformanceRecordIds VARCHAR(MAX)
		)

	INSERT INTO #tblPARecord (
		PARecordId
		,EmployeeId
		,StatusId
		)
	SELECT pa.PARecordId
		,pa.EmployeeId
		,pa.StatusId
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
			SELECT process.PARecordId AS '@parid'
				,process.ProcessId AS '@pid'
				,process.EvaluatorId AS '@eid'
				,process.GradeId AS '@gid'
				,process.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS process WITH (NOLOCK)
				INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
			WHERE process.ProcessId >= @SelfProcessId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

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
	INSERT INTO #tblPAProcess (
		PARecordId
		,EvaluatorId
		,ProcessId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,pm.ProjectManagerId
		,@PMProcessId
		,lg.GradeId
		,STUFF((
				SELECT @separator + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
				FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
				WHERE prcmt.CreatorId = pm.ProjectManagerId
					AND prcmt.EmployeeId = pa.EmployeeId
					AND prcmt.TypeId = @CheckinTypeId
					AND prcmt.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND prcmt.RoleId = @PMProcessId
				ORDER BY prcmt.CreatedDate DESC
				FOR XML PATH('')
				), 1, 1, N'')
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
				AND process.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			)

	-- Capture for Self, Reviewer
	INSERT INTO #tblPAProcess (
		PARecordId
		,EvaluatorId
		,ProcessId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,process.EvaluatorId
		,process.ProcessId
		,lg.GradeId
		,STUFF((
				SELECT @separator + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
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
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId IN (@SelfProcessId, @ReviewerProcessId)
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

	-- Capture for Evaluator
	INSERT INTO #tblPAProcess (
		PARecordId
		,EvaluatorId
		,ProcessId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,process.EvaluatorId
		,process.ProcessId
		,lg.GradeId
		,STUFF((
				SELECT @separator + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
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
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
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

	-- Insert new PM Process
	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
	FROM #tblPAProcess AS process
		INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
		AND pa.StatusId = @NotGenerated
	WHERE process.ProcessId = @PMProcessId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS process WITH (NOLOCK)
			WHERE pa.PARecordId = process.PARecordId
				AND process.ProcessId = @PMProcessId
				AND process.EvaluatorId = process.EvaluatorId
			)

	UPDATE process
	SET process.GradeId = tblprocess.GradeId
		,process.RelatedPerformanceRecordIds = tblprocess.RelatedPerformanceRecordIds
	FROM dbo.PAProcess AS process WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
		AND pa.StatusId = @NotGenerated
		INNER JOIN #tblPAProcess AS tblprocess ON pa.PARecordId = tblprocess.PARecordId
		AND process.EvaluatorId = tblprocess.EvaluatorId
		AND process.ProcessId = tblprocess.ProcessId

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
		UPDATE process
		SET process.GradeId = tblprocess.GradeId
			,process.RelatedPerformanceRecordIds = tblprocess.RelatedPerformanceRecordIds
		FROM dbo.PAProcess AS process WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
			INNER JOIN #tblPAProcess AS tblprocess ON pa.PARecordId = tblprocess.PARecordId
			AND process.EvaluatorId = tblprocess.EvaluatorId
			AND process.ProcessId = tblprocess.ProcessId
	END

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
	SET @xml = (
			SELECT process.PARecordId AS '@parid'
				,process.ProcessId AS '@pid'
				,process.EvaluatorId AS '@eid'
				,process.GradeId AS '@gid'
				,process.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS process WITH (NOLOCK)
				INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
			WHERE process.ProcessId >= @SelfProcessId
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

	DROP TABLE #tblPAProcess
END
