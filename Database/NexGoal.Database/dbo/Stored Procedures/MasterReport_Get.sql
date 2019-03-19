CREATE PROCEDURE [dbo].[MasterReport_Get] @PeriodId INT
	,@ActorId INT
	,@GroupIds VARCHAR(MAX) = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
AS
/*
Created: 20190115@Elton.ton
Task: get periods
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
	DECLARE @WorkingFromDate DATE
		,@WorkingToDate DATE

	SELECT @WorkingFromDate = DisciplineStartDate
		,@WorkingToDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT val
	INTO #group
	FROM dbo.func_SplitString(@GroupIds, ',')

	SELECT val
	INTO #employee
	FROM dbo.func_SplitString(@EmployeeIds, ',')

	SELECT pst.EmployeeId
		,SUM(pst.[Hours]) AS TotalSpentTime
	INTO #employeespenttime
	FROM dbo.ProjectSpentTime AS pst WITH (NOLOCK)
	WHERE pst.SpentOn BETWEEN @WorkingFromDate
			AND @WorkingToDate
		AND EXISTS (
			SELECT 1
			FROM #employee AS e
			WHERE e.val = pst.EmployeeId
			)
	GROUP BY pst.EmployeeId

	SELECT ec.EmpId
		,SUM(CASE 
				WHEN ISNULL(ec.IsOffDay, 0) = 0
					AND ISNULL(ec.IsHoliday, 0) = 0 THEN 1
				ELSE 0
				END) AS WorkingDay
		,SUM(CASE 
				WHEN ea.Absence IS NOT NULL
					AND ea.Absence = 0
					AND ec.IsLeaveDay = 0 THEN 1
				WHEN ea.Absence IS NOT NULL
					AND ea.Absence = 0
					AND ec.IsLeaveDay = 1
					AND ec.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) ActualWorkingDay
	INTO #workingday
	FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance AS ea WITH (NOLOCK) ON ec.EmpId = ea.EmpId
		AND ec.WorkDate = ea.AttendanceDate
	WHERE ec.WorkDate BETWEEN @WorkingFromDate
			AND @WorkingToDate
		AND EXISTS (
			SELECT 1
			FROM #employee AS e
			WHERE e.val = ec.EmpId
			)
	GROUP BY ec.EmpId

	SELECT e.EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
		,g.[Name] AS GroupName
		,jt.[Name] AS JobTitle
		,par.PARecordId
		,par.OriginalGradeId
		,par.GradeId
		,par.Comment
		,par.Toeic
		,par.ToeicPolicy
		,paf.Contribution
		,paf.BonusGiven
		,paf.IsReadyPromotion
		,l.[Description] AS CurrentObjective
		,n.[Description] AS NextObjective
		,wd.WorkingDay
		,wd.ActualWorkingDay
		,est.TotalSpentTime
		,(CASE 
			WHEN e.EmployeeId = g.LeaderId THEN CAST(1 AS BIT)
			ELSE CAST(0 AS BIT) END
			) AS IsGroupLeader
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN #employeespenttime AS est ON est.EmployeeId = par.EmployeeId
		INNER JOIN #workingday AS wd ON wd.EmpId = par.EmployeeId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) se ON par.EmployeeId = se.EmployeeId
		LEFT JOIN dbo.PAFocus AS paf WITH (NOLOCK) ON paf.PeriodId = par.PeriodId
		AND paf.EmployeeId = par.EmployeeId
		LEFT JOIN dbo.EmployeeObjective AS l WITH (NOLOCK) ON l.ObjectiveId = paf.LastObjectiveId
		LEFT JOIN dbo.EmployeeObjective AS n WITH (NOLOCK) ON n.ObjectiveId = paf.NextObjectiveId
	WHERE EXISTS (
			SELECT 1
			FROM #group AS g
			WHERE e.GroupId = g.val
			)
		AND EXISTS (
			SELECT 1
			FROM #employee AS eid
			WHERE e.EmployeeId = eid.val
			)
		AND par.PeriodId = @PeriodId
	ORDER BY se.RankIndex

	SELECT n.NomineeId
		,n.StatusId
		,rr.Value AS [Status]
		,eoq.[Quarter]
		,eoq.[Year]
	FROM dbo.Nomination AS n WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON n.NomineeId = e.EmployeeId
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON n.StatusId = rr.ResourceId
		AND rr.RefName = 'NominationStatus'
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON n.EOQYId = eoq.EOQYId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.Period AS p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
				AND p.DisciplineStartDate <= eoq.EndDateForCommittee
				AND p.DisciplineEndDate >= eoq.EndDateForCommittee
			)
		AND EXISTS (
			SELECT 1
			FROM #group AS g
			WHERE e.GroupId = g.val
			)
		AND EXISTS (
			SELECT 1
			FROM #employee AS eid
			WHERE e.EmployeeId = eid.val
			)
		AND eoq.EndDateForCommittee < @Today

	SELECT evaluator.NickName
		,evaluator.FullName
		,pap.EvaluatorId
		,pr.GradeId
		,pr.Comment
		,pap.ProcessId
		,par.EmployeeId
		,pap.PerformanceRecordId
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.PAProcess AS pap WITH (NOLOCK) ON par.PARecordId = pap.PARecordId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON pap.EvaluatorId = evaluator.EmployeeId
		LEFT JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON pap.PerformanceRecordId = pr.PerformanceRecordId
	WHERE par.PeriodId = @PeriodId
		AND pap.ProcessId <> 1
		AND EXISTS (
			SELECT 1
			FROM #group AS g
			WHERE e.GroupId = g.val
			)
		AND EXISTS (
			SELECT 1
			FROM #employee AS eid
			WHERE e.EmployeeId = eid.val
			)

	DROP TABLE #group

	DROP TABLE #employee

	DROP TABLE #employeespenttime

	DROP TABLE #workingday
END
