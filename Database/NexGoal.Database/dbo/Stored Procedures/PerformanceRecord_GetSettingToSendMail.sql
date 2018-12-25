
CREATE PROCEDURE [dbo].[PerformanceRecord_GetSettingToSendMail] @PerformanceRecordIds VARCHAR(MAX)
	,@ActorId INT
AS
/*
Created: 20181127@Tim.Trinh
Task: Get seting to send mail.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@MailType TINYINT = 1
		,@Submitted TINYINT = 3
		,@PeriodId INT
		,@PARejected TINYINT = 5
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	CREATE TABLE #tblPerformanceRecordIds (
		PerformanceRecordId INT
		,EmployeeId INT
		)

	CREATE TABLE #tblResult (
		PerformanceRecordId INT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		,ProcessId TINYINT
		)

	SELECT TOP 1 @PeriodId = PeriodId
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND @Today BETWEEN DisciplineStartDate
			AND DisciplineEndDate
	ORDER BY DisciplineStartDate DESC

	INSERT INTO #tblPerformanceRecordIds (
		PerformanceRecordId
		,EmployeeId
		)
	SELECT val
		,CAST(record.EmployeeIds AS INT)
	FROM dbo.func_SplitString(@PerformanceRecordIds, ',') AS tbl
		INNER JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON ISNUMERIC(tbl.val) = 1
		AND tbl.val = record.PerformanceRecordId
		AND record.StatusId = @Submitted

	INSERT INTO #tblResult (
		PerformanceRecordId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ProcessId
		)
	SELECT tbl.PerformanceRecordId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,@SelfProcessId
	FROM #tblPerformanceRecordIds AS tbl WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON tbl.EmployeeId = emp.EmployeeId
	WHERE tbl.EmployeeId <> @ActorId

	INSERT INTO #tblResult (
		PerformanceRecordId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ProcessId
		)
	SELECT record.PerformanceRecordId
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,process.ProcessId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId IN (@SelfProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		AND process.EvaluatorId <> @ActorId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		INNER JOIN #tblPerformanceRecordIds AS record WITH (NOLOCK) ON pa.EmployeeId = record.EmployeeId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON process.EvaluatorId = evaluator.EmployeeId
	WHERE pa.StatusId <> @PARejected
		AND NOT EXISTS (
			SELECT 1
			FROM #tblResult AS tbl WITH (NOLOCK)
			WHERE record.PerformanceRecordId = tbl.PerformanceRecordId
				AND process.EvaluatorId = tbl.EvaluatorId
				AND process.ProcessId = tbl.ProcessId
			)

	INSERT INTO #tblResult (
		PerformanceRecordId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ProcessId
		)
	SELECT record.PerformanceRecordId
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,efs.ProcessId
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN #tblPerformanceRecordIds AS record WITH (NOLOCK) ON efs.EmployeeId = record.EmployeeId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON efs.EvaluatorId = evaluator.EmployeeId
	WHERE efs.ProcessId IN (@SelfProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		AND efs.EvaluatorId <> @ActorId
		AND NOT EXISTS (
			SELECT 1
			FROM #tblResult AS tbl WITH (NOLOCK)
			WHERE record.PerformanceRecordId = tbl.PerformanceRecordId
				AND efs.EvaluatorId = tbl.EvaluatorId
				AND efs.ProcessId = tbl.ProcessId
			)

	INSERT INTO #tblResult (
		PerformanceRecordId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ProcessId
		)
	SELECT record.PerformanceRecordId
		,evaluator.EmployeeId
		,evaluator.NickName
		,evaluator.FullName
		,evaluator.Email
		,gfs.ProcessId
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
		INNER JOIN #tblPerformanceRecordIds AS record WITH (NOLOCK) ON emp.EmployeeId = record.EmployeeId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON gfs.EvaluatorId = evaluator.EmployeeId
	WHERE gfs.ProcessId IN (@SelfProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		AND gfs.EvaluatorId <> @ActorId
		AND NOT EXISTS (
			SELECT 1
			FROM #tblResult AS tbl WITH (NOLOCK)
			WHERE record.PerformanceRecordId = tbl.PerformanceRecordId
				AND gfs.EvaluatorId = tbl.EvaluatorId
				AND gfs.ProcessId = tbl.ProcessId
			)

	SELECT tbl.PerformanceRecordId
		,tbl.EvaluatorId
		,tbl.Nickname
		,tbl.FullName
		,tbl.Email
		,tbl.ProcessId
	FROM #tblResult AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.MailConfig AS config WITH (NOLOCK) ON tbl.EvaluatorId = config.EmployeeId
	WHERE ISNULL(config.[Type], @MailType) = @MailType
		AND ISNULL(config.Notify, 1) = 1
	ORDER BY tbl.PerformanceRecordId
		,tbl.ProcessId

	DROP TABLE #tblPerformanceRecordIds

	DROP TABLE #tblResult
END