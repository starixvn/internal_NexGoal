
CREATE PROCEDURE [dbo].[Period_InsertEmployeeSettings] @PeriodId INT
	,@EmployeeId INT = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180827@Elton.Ton
Task: insert employee setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PeriodStartDate DATE
		,@PeriodEndDate DATE
		,@SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
	DECLARE @paRecordTbl TABLE (
		PARecordId INT
		,EmployeeId INT
		)
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SELECT @PeriodStartDate = StartDate
		,@PeriodEndDate = EndDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	INSERT INTO dbo.PARecord (
		EmployeeId
		,PeriodId
		,StatusId
		)
	OUTPUT INSERTED.PARecordId
		,INSERTED.EmployeeId
	INTO @paRecordTbl
	SELECT e.EmployeeId
		,@PeriodId
		,1 -- not generated
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE (
			e.EmployeeId = @EmployeeId
			OR EXISTS (
				SELECT 1
				FROM dbo.func_SplitString(@GroupIds, ',') AS g
				WHERE g.val = e.GroupId
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PARecord AS par WITH (NOLOCK)
			WHERE par.EmployeeId = e.EmployeeId
				AND par.PeriodId = @PeriodId
			)
		AND e.[Enabled] = 1
		AND e.IsDeleted = 0
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
				WHERE efs.EmployeeId = e.EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
				WHERE gfs.GroupId = e.GroupId
				)
			)
	ORDER BY e.GroupId
		,e.EmployeeId

	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,StartDate
		,EndDate
		,[Priority]
		)
	SELECT par.PARecordId
		,efs.ProcessId
		,efs.EvaluatorId
		,CASE 
			WHEN efs.ProcessId = @ReviewerProcessId THEN @PeriodStartDate
			ELSE NULL
			END
		,CASE 
			WHEN efs.ProcessId = @ReviewerProcessId THEN @PeriodEndDate
			ELSE NULL
			END
		,efs.[Priority]
	FROM @paRecordTbl AS par
		INNER JOIN dbo.EmployeeFlowSetting AS efs WITH (NOLOCK) ON par.EmployeeId = efs.EmployeeId
	
	UNION
	SELECT par.PARecordId
		,@SelfProcessId
		,par.EmployeeId
		,NULL
		,NULL
		,0
	FROM @paRecordTbl AS par

	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,StartDate
		,EndDate
		,[Priority]
		)
	SELECT par.PARecordId
		,gfs.ProcessId
		,gfs.EvaluatorId
		,CASE 
			WHEN gfs.ProcessId = @ReviewerProcessId THEN @PeriodStartDate
			ELSE NULL
			END
		,CASE 
			WHEN gfs.ProcessId = @ReviewerProcessId THEN @PeriodEndDate
			ELSE NULL
			END
		,gfs.[Priority]
	FROM @paRecordTbl AS par
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = par.EmployeeId
		INNER JOIN dbo.GroupFlowSetting AS gfs WITH (NOLOCK) ON e.GroupId = gfs.GroupId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
			WHERE efs.EmployeeId = par.EmployeeId
			)

	SET @xml = (
			SELECT pa.PARecordId AS '@paid'
				,pa.EmployeeId AS '@empid'
				,pa.PeriodId AS '@pid'
				,pa.StatusId AS '@psid'
				,(
					SELECT process.ProcessId AS '@proid'
						,process.EvaluatorId AS '@evaid'
						,process.[Priority] AS '@pri'
						,process.StartDate AS '@sdate'
						,process.EndDate AS '@edate'
					FROM dbo.PAProcess AS process WITH (NOLOCK)
					WHERE process.PARecordId = pa.PARecordId
					FOR XML PATH('z')
						,TYPE
					)
			FROM dbo.PARecord AS pa WITH (NOLOCK)
			WHERE pa.PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'Period_InsertEmployeeSettings'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@Duration = @Duration
END
