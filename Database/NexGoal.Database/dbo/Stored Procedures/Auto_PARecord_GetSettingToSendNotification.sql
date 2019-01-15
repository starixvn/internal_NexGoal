
CREATE PROCEDURE [dbo].[Auto_PARecord_GetSettingToSendNotification] @ReceiveType TINYINT
AS
/*
Created: 20190107@Tim.Trinh
Task: get setting to send notification.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@NotificationType TINYINT = 2
		,@InProgress TINYINT = 3
		,@Closed TINYINT = 4
		,@StartMode TINYINT = 1
		,@RemindMode TINYINT = 2
		,@RemindStartEvaluationMode TINYINT = 3
		,@EndEvaluationMode TINYINT = 4
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 5
		-- receive type
		,@ReceiveMail TINYINT = 1
		,@ReceiveNotification TINYINT = 2

	CREATE TABLE #tblProcessSetting (
		PeriodId INT
		,PerformanceRecordId INT
		,NotificationMode TINYINT
		,ProcessId TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		,StartDate DATE
		,EndDate DATE
		)

	INSERT INTO #tblProcessSetting (
		PeriodId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		)
	SELECT data.PeriodId
		,@RemindStartEvaluationMode AS NotificationMode
		,MAX(data.ProcessId) AS ProcessId
		,data.EvaluatorId
		,data.NickName
		,data.FullName
		,data.Email
	FROM (
		SELECT period.PeriodId
			,efs.ProcessId
			,efs.EvaluatorId
			,emp.NickName
			,emp.FullName
			,emp.Email
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.EmployeeFlowSetting AS efs WITH (NOLOCK) ON emp.EmployeeId = efs.EvaluatorId
			AND efs.ProcessId > @SelfProcessId
			,dbo.Period AS period WITH (NOLOCK)
		WHERE emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND DATEADD(DAY, - 7, period.StartDate) = @Today
		
		UNION
		SELECT period.PeriodId
			,gfs.ProcessId
			,gfs.EvaluatorId
			,emp.NickName
			,emp.FullName
			,emp.Email
		FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gfs.EvaluatorId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
			AND gr.IsDeleted = 0
			,dbo.Period AS period WITH (NOLOCK)
		WHERE gfs.ProcessId > @SelfProcessId
			AND DATEADD(DAY, - 7, period.StartDate) = @Today
		
		UNION
		SELECT period.PeriodId
			,@PMProcessId AS ProcessId
			,emp.EmployeeId AS EvaluatorId
			,emp.NickName
			,emp.FullName
			,emp.Email
		FROM dbo.Employee AS emp WITH (NOLOCK)
			,dbo.Period AS period WITH (NOLOCK)
		WHERE emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND DATEADD(DAY, - 7, period.StartDate) = @Today
			AND EXISTS (
				SELECT 1
				FROM dbo.PMSetting AS pmsetting WITH (NOLOCK)
				WHERE pmsetting.ProjectManagerId = emp.EmployeeId
				)
		) AS data
	GROUP BY data.PeriodId
		,data.EvaluatorId
		,data.NickName
		,data.FullName
		,data.Email

	INSERT INTO #tblProcessSetting (
		PeriodId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		)
	SELECT period.PeriodId
		,@RemindStartEvaluationMode AS NotificationMode
		,@SelfProcessId AS ProcessId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		,dbo.Period AS period WITH (NOLOCK)
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND DATEADD(DAY, - 7, period.StartDate) = @Today
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
				WHERE efs.EmployeeId = emp.EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
				WHERE gfs.GroupId = gr.GroupId
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tblProcessSetting AS tbl WITH (NOLOCK)
			WHERE period.PeriodId = tbl.PeriodId
				AND emp.EmployeeId = tbl.EvaluatorId
			)

	INSERT INTO #tblProcessSetting (
		PeriodId
		,PerformanceRecordId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,StartDate
		,EndDate
		)
	SELECT DISTINCT period.PeriodId
		,NULL AS PerformanceRecordId
		,(CASE 
			WHEN process.StartDate = @Today THEN @StartMode
			ELSE @RemindMode END
			) AS NotificationMode
		,process.ProcessId
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,process.StartDate
		,process.EndDate
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @ReviewerProcessId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.PeriodProcessSetting AS ps WITH (NOLOCK) ON period.PeriodId = ps.PeriodId
		AND ps.ProcessId = @ReviewerProcessId
	WHERE period.IsActive = 1
		AND (
			process.StartDate = @Today
			OR DATEADD(DAY, - ps.RemindDay, process.EndDate) = @Today
			)
	
	UNION
	SELECT DISTINCT period.PeriodId
		,pa.PerformanceRecordId
		,@EndEvaluationMode AS NotificationMode
		,process.ProcessId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,NULL AS StartDate
		,NULL AS EndDate
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND pa.StatusId IN (@InProgress, @Closed)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @ReviewerProcessId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
	WHERE period.IsActive = 1
		AND period.EndDate < @Today
		AND process.EndDate < @Today
		AND (
			DATEADD(DAY, 1, period.EndDate) = @Today
			OR DATEADD(DAY, 1, process.EndDate) = @Today
			)

	SELECT period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
	FROM dbo.Period AS period WITH (NOLOCK)
	WHERE period.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM #tblProcessSetting AS tbl WITH (NOLOCK)
				LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.EvaluatorId = setting.EmployeeId
			WHERE period.PeriodId = tbl.PeriodId
				AND ISNULL(setting.TypeId, @NotificationType) = @NotificationType
				AND (
					(
						@ReceiveType = @ReceiveMail
						AND ISNULL(setting.ReceiveEmail, 1) = 1
						)
					OR (
						@ReceiveType = @ReceiveNotification
						AND ISNULL(setting.ReceiveNotification, 1) = 1
						)
					)
			)

	SELECT PeriodId
		,PerformanceRecordId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,StartDate
		,EndDate
	FROM #tblProcessSetting AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.EvaluatorId = setting.EmployeeId
	WHERE ISNULL(setting.TypeId, @NotificationType) = @NotificationType
		AND (
			(
				@ReceiveType = @ReceiveMail
				AND ISNULL(setting.ReceiveEmail, 1) = 1
				)
			OR (
				@ReceiveType = @ReceiveNotification
				AND ISNULL(setting.ReceiveNotification, 1) = 1
				)
			)

	DROP TABLE #tblProcessSetting
END