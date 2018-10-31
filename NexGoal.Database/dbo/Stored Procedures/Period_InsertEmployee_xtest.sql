
CREATE PROCEDURE [dbo].[Period_InsertEmployee_xtest] @PeriodId INT
	,@EmployeeId INT = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180827@Elton.Ton
Task: insert employee to period
DB: bodb_NexGoal
Original:

Revisions:
    20180830@Tim.Trinh: refactor
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TargetTypeId TINYINT
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tblGroupId (GroupId INT)

	CREATE TABLE #tblEmployeeId (EmployeeId INT)

	CREATE TABLE #tblPARecord (
		PARecordId INT
		,EmployeeId INT
		)

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	INSERT INTO #tblGroupId (GroupId)
	SELECT DISTINCT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblEmployeeId (EmployeeId)
	SELECT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN #tblGroupId AS tblid WITH (NOLOCK) ON gr.GroupId = tblid.GroupId
		LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON emp.EmployeeId = pa.EmployeeId
		AND pa.PeriodId = @PeriodId
	WHERE (
			emp.EmployeeId = @EmployeeId
			OR tblid.GroupId IS NOT NULL
			)
		AND pa.EmployeeId IS NULL
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1

	-- employee flow --
	INSERT INTO dbo.PARecord (
		EmployeeId
		,PeriodId
		,StatusId
		)
	OUTPUT INSERTED.PARecordId
		,INSERTED.EmployeeId
	INTO #tblPARecord
	SELECT DISTINCT efs.EmployeeId
		,@PeriodId
		,1
	FROM #tblEmployeeId AS empid WITH (NOLOCK)
		INNER JOIN dbo.EmployeeFlowSetting AS efs WITH (NOLOCK) ON empid.EmployeeId = efs.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON efs.FlowTemplateId = ft.FlowTemplateId
		AND ft.TargetTypeId = @TargetTypeId

	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT tblpa.PARecordId
		,efs.ProcessId
		,efs.EvaluatorId
		,efs.[Priority]
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON efs.FlowTemplateId = ft.FlowTemplateId
		AND ft.TargetTypeId = @TargetTypeId
		INNER JOIN #tblPARecord AS tblpa WITH (NOLOCK) ON efs.EmployeeId = tblpa.EmployeeId

	-- group flow --
	INSERT INTO dbo.PARecord (
		EmployeeId
		,PeriodId
		,StatusId
		)
	OUTPUT INSERTED.PARecordId
		,INSERTED.EmployeeId
	INTO #tblPARecord
	SELECT DISTINCT emp.EmployeeId
		,@PeriodId
		,1
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
		INNER JOIN #tblEmployeeId AS empid WITH (NOLOCK) ON emp.EmployeeId = empid.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON gfs.FlowTemplateId = ft.FlowTemplateId
		AND ft.TargetTypeId = @TargetTypeId
		LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON emp.EmployeeId = pa.EmployeeId
		AND pa.PeriodId = @PeriodId
	WHERE pa.EmployeeId IS NULL

	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT tblpa.PARecordId
		,gfs.ProcessId
		,gfs.EvaluatorId
		,gfs.[Priority]
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
		INNER JOIN #tblPARecord AS tblpa WITH (NOLOCK) ON emp.EmployeeId = tblpa.EmployeeId
		INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON gfs.FlowTemplateId = ft.FlowTemplateId
		AND ft.TargetTypeId = @TargetTypeId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PARecord AS pa WITH (NOLOCK)
				INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
			WHERE pa.PARecordId = tblpa.PARecordId
			)

	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT PARecordId
		,2
		,EmployeeId
		,0
	FROM #tblPARecord WITH (NOLOCK)

	-- write log after insert
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
				INNER JOIN #tblPARecord AS tblpa WITH (NOLOCK) ON pa.PARecordId = tblpa.PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	DROP TABLE #tblGroupId

	DROP TABLE #tblEmployeeId

	DROP TABLE #tblPARecord
END
