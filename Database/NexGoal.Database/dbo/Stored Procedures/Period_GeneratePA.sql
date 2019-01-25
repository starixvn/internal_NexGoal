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
		,@disciplineStartDate DATE
		,@disciplineEndDate DATE
		,@PMRoleId TINYINT = 6
	-- process
	DECLARE @SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
	-- Performance Status
	DECLARE @PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	DECLARE @selectedPARecordTbl TABLE (PARecordId INT PRIMARY KEY)
	DECLARE @paRecordTbl TABLE (
		PARecordId INT PRIMARY KEY
		,EmployeeId INT
		)

	CREATE TABLE #performanceRecordTbl (
		PerFormanceRecordId INT PRIMARY KEY
		,EmployeeId INT
		,EvaluatorId INT
		)

	DECLARE @today DATETIME = GETDATE()
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	INSERT INTO @selectedPARecordTbl (PARecordId)
	SELECT val
	FROM dbo.func_SplitString(@PARecordIds, ',')

	INSERT INTO @paRecordTbl (
		PARecordId
		,EmployeeId
		)
	SELECT par.PARecordId
		,par.EmployeeId
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN @selectedPARecordTbl AS spari ON spari.PARecordId = par.PARecordId
	WHERE par.PeriodId = @PeriodId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
			WHERE pap.ProcessId >= @SelfProcessId
				AND pap.PerformanceRecordId IS NOT NULL
				AND pap.PARecordId = par.PARecordId
			)

	SELECT @periodStartDate = p.StartDate
		,@periodEndDate = p.EndDate
		,@disciplineStartDate = p.DisciplineStartDate
		,@disciplineEndDate = p.DisciplineEndDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pr.PerformanceRecordId AS '@pri'
				,pr.EmployeeId AS '@pei'
				,pr.GradeId AS '@pgi'
				,pr.Comment AS '@pc'
				,pr.CreatedDate AS '@pcd'
				,pr.ModifiedDate AS '@pmd'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
				INNER JOIN @selectedPARecordTbl AS spari ON pap.PARecordId = spari.PARecordId
				LEFT JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON pap.PerformanceRecordId = pr.PerformanceRecordId
			WHERE pap.ProcessId >= @SelfProcessId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	IF EXISTS (
			SELECT 1
			FROM @paRecordTbl
			)
	BEGIN
		-- Capture For PM
		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,GradeId
			,Comment
			,CreatedDate
			,TypeId
			)
		OUTPUT INSERTED.PerformanceRecordId
			,INSERTED.EmployeeId
			,INSERTED.CreatorId
		INTO #performanceRecordTbl(PerFormanceRecordId, EmployeeId, EvaluatorId)
		SELECT par.EmployeeId
			,lg.CreatorId
			,lg.GradeId
			,STUFF((
					SELECT CHAR(10) + prcmt.Comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeId = par.EmployeeId
						AND prcmt.CreatorId = lg.CreatorId
						AND prcmt.TypeId = 1
						AND prcmt.CreatedDate BETWEEN @disciplineStartDate
							AND @disciplineEndDate
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, '')
			,GETDATE()
			,2
		FROM @paRecordTbl AS par
			CROSS APPLY (
			SELECT pr.CreatorId
				,pr.GradeId
			FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
				LEFT JOIN dbo.PerformanceRecord AS pr1 WITH (NOLOCK) ON pr.CreatorId = pr1.CreatorId
				AND pr.EmployeeId = pr1.EmployeeId
				AND pr.CreatedDate < pr1.CreatedDate
				AND pr.TypeId = pr1.TypeId
				AND pr1.CreatedDate BETWEEN @disciplineStartDate
					AND @disciplineEndDate
				AND pr1.TypeId = 1
			WHERE pr1.CreatedDate IS NULL
				AND pr.TypeId = 1
				AND pr.CreatedDate BETWEEN @disciplineStartDate
					AND @disciplineEndDate
				AND pr.EmployeeId = par.EmployeeId
				AND pr.CreatorId <> pr.EmployeeId
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.PAProcess AS pap WITH (NOLOCK)
					WHERE pap.PARecordId = par.PARecordId
						AND pap.EvaluatorId = pr.CreatorId
						AND pap.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
					)
				AND EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole er WITH (NOLOCK)
					WHERE er.RoleId = @PMRoleId AND pr.CreatorId = er.EmployeeId
				)
				AND EXISTS (
					SELECT 1
					FROM dbo.PMSetting pms WITH (NOLOCK)
					WHERE pms.ProjectManagerId = pr.CreatorId AND pms.MemberId = par.EmployeeId
				)
			) lg

		-- Capture for Self, Reviewer, Evaluator
		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,GradeId
			,Comment
			,CreatedDate
			,TypeId
			)
		OUTPUT INSERTED.PerformanceRecordId
			,INSERTED.EmployeeId
			,INSERTED.CreatorId
		INTO #performanceRecordTbl(PerFormanceRecordId, EmployeeId, EvaluatorId)
		SELECT par.EmployeeId
			,pap.EvaluatorId
			,lg.GradeId
			,STUFF((
					SELECT CHAR(10) + prcmt.Comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeId = par.EmployeeId
						AND prcmt.CreatorId = lg.CreatorId
						AND prcmt.TypeId = 1
						AND prcmt.CreatedDate BETWEEN @disciplineStartDate
							AND @disciplineEndDate
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, '')
			,GETDATE()
			,2
		FROM @paRecordTbl AS par
			INNER JOIN dbo.PAProcess AS pap WITH (NOLOCK) ON par.PARecordId = pap.PARecordId
			AND pap.ProcessId IN (@SelfProcessId, @EvaluatorProcessId, @ReviewerProcessId)
			CROSS APPLY (
			SELECT TOP 1 pr.CreatorId
				,pr.GradeId
			FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
			WHERE pr.EmployeeId = par.EmployeeId
				AND pr.CreatorId = pap.EvaluatorId
				AND pr.TypeId = 1
				AND pr.CreatedDate <= @disciplineEndDate
			ORDER BY pr.CreatedDate DESC
			) lg

		UPDATE pap
		SET pap.PerformanceRecordId = prt.PerFormanceRecordId
		FROM dbo.PAProcess AS pap WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @paRecordTbl AS par ON pap.PARecordId = par.PARecordId
			INNER JOIN #performanceRecordTbl AS prt ON par.EmployeeId = prt.EmployeeId
			AND pap.EvaluatorId = prt.EvaluatorId
		WHERE pap.ProcessId >= @SelfProcessId

		-- Insert PM Process
		INSERT INTO dbo.PAProcess (
			PARecordId
			,ProcessId
			,EvaluatorId
			,PerformanceRecordId
			,[Priority]
			)
		SELECT par.PARecordId
			,@PMProcessId
			,prt.EvaluatorId
			,prt.PerFormanceRecordId
			,0
		FROM #performanceRecordTbl AS prt
			INNER JOIN @paRecordTbl AS par ON prt.EmployeeId = par.EmployeeId
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.PAProcess AS pap WITH (NOLOCK)
				WHERE pap.PerformanceRecordId = prt.PerFormanceRecordId
				)

		UPDATE par
		SET StatusId = (CASE 
				WHEN @today >= reviewer.StartDate
					AND (
						@today <= reviewer.EndDate
						OR @today <= @periodEndDate
						) THEN @PAInProgress
				WHEN @periodEndDate < @today
					AND reviewer.EndDate < @today THEN @PAClosed
				WHEN @today < reviewer.StartDate THEN @PAOpened
				ELSE par.StatusId END
				)
		FROM dbo.PARecord AS par WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON par.PARecordId = reviewer.PARecordId
			AND reviewer.ProcessId = @ReviewerProcessId
		WHERE EXISTS (
				SELECT 1
				FROM @paRecordTbl AS part
				WHERE par.PARecordId = part.PARecordId
				)
	END

	IF @Override = 1
	BEGIN
		DECLARE @generatedPARecordTbl TABLE (
			PARecordId INT PRIMARY KEY
			,EmployeeId INT
			)

		INSERT INTO @generatedPARecordTbl (
			PARecordId
			,EmployeeId
			)
		SELECT spari.PARecordId
			,par.EmployeeId
		FROM @selectedPARecordTbl AS spari
			INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON par.PARecordId = spari.PARecordId
		WHERE NOT EXISTS (
				SELECT 1
				FROM @paRecordTbl AS part
				WHERE spari.PARecordId = part.PARecordId
				)

		-- update existing records
		UPDATE pr
		SET pr.GradeId = lg.GradeId
			,pr.Comment = STUFF((
					SELECT CHAR(10) + prcmt.Comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeId = pr.EmployeeId
						AND prcmt.CreatorId = pap.EvaluatorId
						AND prcmt.TypeId = 1
						AND (
							pap.ProcessId <> @PMProcessId
							OR prcmt.CreatedDate >= @disciplineStartDate
							)
						AND prcmt.CreatedDate <= @disciplineEndDate
					ORDER BY pr.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, '')
			,pr.CreatorId = pap.EvaluatorId
			,pr.ModifiedDate = GETDATE()
		FROM dbo.PerformanceRecord AS pr WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.PAProcess AS pap WITH (NOLOCK) ON pr.PerformanceRecordId = pap.PerformanceRecordId
			CROSS APPLY (
			SELECT TOP 1 pr1.GradeId
			FROM dbo.PerformanceRecord AS pr1 WITH (NOLOCK)
			WHERE pr1.EmployeeId = pr.EmployeeId
				AND pr1.CreatorId = pap.EvaluatorId
				AND pr1.TypeId = 1
				AND (
					pap.ProcessId <> @PMProcessId
					OR pr.CreatedDate >= @disciplineStartDate
					)
				AND pr1.CreatedDate <= @disciplineEndDate
			ORDER BY pr1.CreatedDate DESC
			) lg
		WHERE EXISTS (
				SELECT 1
				FROM @generatedPARecordTbl AS gpar
				WHERE pap.PARecordId = gpar.PARecordId
				)

		-- Add new PCIs from PM
		DELETE
		FROM #performanceRecordTbl

		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,GradeId
			,Comment
			,CreatedDate
			,TypeId
			)
		OUTPUT INSERTED.PerformanceRecordId
			,INSERTED.EmployeeId
			,INSERTED.CreatorId
		INTO #performanceRecordTbl(PerFormanceRecordId, EmployeeId, EvaluatorId)
		SELECT gpar.EmployeeId
			,lg.CreatorId
			,lg.GradeId
			,STUFF((
					SELECT CHAR(10) + prcmt.Comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeId = gpar.EmployeeId
						AND prcmt.CreatorId = lg.CreatorId
						AND prcmt.TypeId = 1
						AND prcmt.CreatedDate BETWEEN @disciplineStartDate
							AND @disciplineEndDate
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, '')
			,GETDATE()
			,2
		FROM @generatedPARecordTbl AS gpar
			CROSS APPLY (
			SELECT pr.CreatorId
				,pr.GradeId
			FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
				LEFT JOIN dbo.PerformanceRecord AS pr1 WITH (NOLOCK) ON pr.CreatorId = pr1.CreatorId
				AND pr.EmployeeId = pr1.EmployeeId
				AND pr.CreatedDate < pr1.CreatedDate
				AND pr.TypeId = pr1.TypeId
				AND pr1.CreatedDate BETWEEN @disciplineStartDate
					AND @disciplineEndDate
				AND pr1.TypeId = 1
			WHERE pr1.CreatedDate IS NULL
				AND pr.TypeId = 1
				AND pr.CreatedDate BETWEEN @disciplineStartDate
					AND @disciplineEndDate
				AND pr.EmployeeId = gpar.EmployeeId
				AND pr.CreatorId <> pr.EmployeeId
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.PAProcess AS pap WITH (NOLOCK)
					WHERE pap.PARecordId = gpar.PARecordId
						AND pap.EvaluatorId = pr.CreatorId
						AND pap.ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
					)
				AND EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole er WITH (NOLOCK)
					WHERE er.RoleId = @PMRoleId AND pr.CreatorId = er.EmployeeId
				)
				AND EXISTS (
					SELECT 1
					FROM dbo.PMSetting pms WITH (NOLOCK)
					WHERE pms.ProjectManagerId = pr.CreatorId AND pms.MemberId = gpar.EmployeeId
				)
			) lg

		-- Insert PM Process
		INSERT INTO dbo.PAProcess (
			PARecordId
			,ProcessId
			,EvaluatorId
			,PerformanceRecordId
			,[Priority]
			)
		SELECT gpar.PARecordId
			,@PMProcessId
			,prt.EvaluatorId
			,prt.PerFormanceRecordId
			,0
		FROM #performanceRecordTbl AS prt
			INNER JOIN @generatedPARecordTbl AS gpar ON prt.EmployeeId = gpar.EmployeeId

		DELETE
		FROM #performanceRecordTbl

		-- Capture for Self, Reviewer, Evaluator
		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,GradeId
			,Comment
			,CreatedDate
			,TypeId
			)
		OUTPUT INSERTED.PerformanceRecordId
			,INSERTED.EmployeeId
			,INSERTED.CreatorId
		INTO #performanceRecordTbl(PerFormanceRecordId, EmployeeId, EvaluatorId)
		SELECT gpar.EmployeeId
			,pap.EvaluatorId
			,lg.GradeId
			,STUFF((
					SELECT CHAR(10) + prcmt.Comment
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.EmployeeId = gpar.EmployeeId
						AND prcmt.CreatorId = lg.CreatorId
						AND prcmt.TypeId = 1
						AND prcmt.CreatedDate BETWEEN @disciplineStartDate
							AND @disciplineEndDate
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, '')
			,GETDATE()
			,2
		FROM @generatedPARecordTbl AS gpar
			INNER JOIN dbo.PAProcess AS pap WITH (NOLOCK) ON gpar.PARecordId = pap.PARecordId
			AND pap.ProcessId IN (@SelfProcessId, @EvaluatorProcessId, @ReviewerProcessId)
			AND pap.PerformanceRecordId IS NULL
			CROSS APPLY (
			SELECT TOP 1 pr.CreatorId
				,pr.GradeId
			FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
			WHERE pr.EmployeeId = gpar.EmployeeId
				AND pr.CreatorId = pap.EvaluatorId
				AND pr.TypeId = 1
				AND pr.CreatedDate <= @disciplineEndDate
			ORDER BY pr.CreatedDate DESC
			) lg

		UPDATE pap
		SET pap.PerformanceRecordId = pr.PerFormanceRecordId
		FROM dbo.PAProcess pap WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @generatedPARecordTbl gpar ON pap.PARecordId = gpar.PARecordId
			INNER JOIN #performanceRecordTbl pr ON pap.EvaluatorId = pr.EvaluatorId
			AND gpar.EmployeeId = pr.EmployeeId
		WHERE pap.PerformanceRecordId IS NULL
			AND pap.ProcessId IN (@SelfProcessId, @EvaluatorProcessId, @ReviewerProcessId)
	END

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pr.PerformanceRecordId AS '@pri'
				,pr.EmployeeId AS '@pei'
				,pr.GradeId AS '@pgi'
				,pr.Comment AS '@pc'
				,pr.CreatedDate AS '@pcd'
				,pr.ModifiedDate AS '@pmd'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
				INNER JOIN @selectedPARecordTbl AS spari ON pap.PARecordId = spari.PARecordId
				LEFT JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON pap.PerformanceRecordId = pr.PerformanceRecordId
			WHERE pap.ProcessId >= @SelfProcessId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'Period_GeneratePA'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@Duration = @Duration

	DROP TABLE #performanceRecordTbl
END
