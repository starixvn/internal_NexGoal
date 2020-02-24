
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
		,@PARejected TINYINT = 5
		,@SelfProcessId TINYINT = 2
	DECLARE @WorkingFromDate DATE
		,@WorkingToDate DATE
	DECLARE @tblGroupId TABLE (GroupId INT)
	DECLARE @tblEmployeeId TABLE (EmpId INT)
	DECLARE @tblPARecordId TABLE (PARecordId INT)

	SELECT @WorkingFromDate = DisciplineStartDate
		,@WorkingToDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	INSERT INTO @tblGroupId (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')

	INSERT INTO @tblEmployeeId (EmpId)
	SELECT val
	FROM dbo.func_SplitString(@EmployeeIds, ',')

	INSERT INTO @tblPARecordId (PARecordId)
	SELECT par.PARecordId
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE par.StatusId <> @PARejected
		AND EXISTS (
			SELECT 1
			FROM @tblGroupId AS g
			WHERE e.GroupId = g.GroupId
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS eid
			WHERE e.EmployeeId = eid.EmpId
			)
		AND par.PeriodId = @PeriodId

	SELECT pst.EmployeeId
		,SUM(pst.[Hours]) AS TotalSpentTime
	INTO #employeespenttime
	FROM dbo.ProjectSpentTime AS pst WITH (NOLOCK)
	WHERE pst.SpentOn BETWEEN @WorkingFromDate
			AND @WorkingToDate
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS e
			WHERE e.EmpId = pst.EmployeeId
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
			FROM @tblEmployeeId AS e
			WHERE e.EmpId = ec.EmpId
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
		INNER JOIN #workingday AS wd ON wd.EmpId = par.EmployeeId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) se ON par.EmployeeId = se.EmployeeId
		LEFT JOIN #employeespenttime AS est ON est.EmployeeId = par.EmployeeId
		LEFT JOIN dbo.PAFocus AS paf WITH (NOLOCK) ON paf.PeriodId = par.PeriodId
		AND paf.EmployeeId = par.EmployeeId
		LEFT JOIN dbo.EmployeeObjective AS l WITH (NOLOCK) ON l.ObjectiveId = paf.LastObjectiveId
		LEFT JOIN dbo.EmployeeObjective AS n WITH (NOLOCK) ON n.ObjectiveId = paf.NextObjectiveId
	WHERE EXISTS (
			SELECT 1
			FROM @tblPARecordId AS tbl
			WHERE par.PARecordId = tbl.PARecordId
			)
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
			FROM @tblGroupId AS g
			WHERE e.GroupId = g.GroupId
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS eid
			WHERE e.EmployeeId = eid.EmpId
			)
		AND eoq.EndDateForCommittee < @Today

	CREATE TABLE #tblRelatedPerformanceRecord (
		PARecordId INT
		,ProcessId TINYINT
		,PerformanceRecordId INT
		)

	SELECT par.PARecordId
		,pap.ProcessId
		,pap.EvaluatorId
		,evaluator.FullName
		,evaluator.NickName
		,pap.GradeId
		,pap.RelatedPerformanceRecordIds
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.PAProcess AS pap WITH (NOLOCK) ON par.PARecordId = pap.PARecordId
		AND pap.ProcessId > 1
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON pap.EvaluatorId = evaluator.EmployeeId
	WHERE EXISTS (
			SELECT 1
			FROM @tblPARecordId AS tbl
			WHERE par.PARecordId = tbl.PARecordId
			)

	INSERT INTO #tblRelatedPerformanceRecord (
		PARecordId
		,ProcessId
		,PerformanceRecordId
		)
	SELECT pa.PARecordId
		,process.ProcessId
		,record.val
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId >= @SelfProcessId
		CROSS APPLY dbo.func_SplitString(process.RelatedPerformanceRecordIds, ',') AS record

	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.CreatedDate
		,record.RoleId
		,record.TypeId
		,creator.EmployeeId
		,creator.Code
		,creator.NickName
		,creator.FullName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.PerformanceRecordId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId

	SELECT reaction.PerformanceRecordId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON reaction.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC

	SELECT fb.PRFeedbackId
		,fb.PerformanceRecordId
		,fb.RoleId
		,fb.Content
		,fb.CreatedDate
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedback AS fb WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON fb.CreatorId = creator.EmployeeId

	SELECT reaction.PRFeedbackId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.PRFeedback AS fb WITH (NOLOCK) ON reaction.PRFeedbackId = fb.PRFeedbackId
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC

	DROP TABLE #employeespenttime

	DROP TABLE #workingday

	DROP TABLE #tblRelatedPerformanceRecord
END
