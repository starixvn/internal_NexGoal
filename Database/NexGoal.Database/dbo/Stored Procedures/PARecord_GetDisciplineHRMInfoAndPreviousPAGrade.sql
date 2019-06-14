
CREATE PROCEDURE [dbo].[PARecord_GetDisciplineHRMInfoAndPreviousPAGrade] @PARecordId INT, @ActorId INT
AS
/*
Created: 20190125@Oliver.Dang
Task: get discipline, HRM detail and last grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	--Get Discipline
	DECLARE @EmployeeId INT
		,@CurrentPeriodId INT
		,@FromDate DATETIME
		,@ToDate DATETIME

	SELECT @EmployeeId = pa.EmployeeId
		,@CurrentPeriodId = period.PeriodId
		,@FromDate = DisciplineStartDate
		,@ToDate = DisciplineEndDate
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON period.PeriodId = pa.PeriodId
	WHERE pa.PARecordId = @PARecordId

	SELECT DisciplineId
		,[Description]
		,EffectiveDate
	FROM dbo.Discipline WITH (NOLOCK)
	WHERE EmployeeId = @EmployeeId
		AND EffectiveDate BETWEEN @FromDate
			AND @ToDate
		AND IsActive = 1
	ORDER BY EffectiveDate DESC
		,CreatedDate


	CREATE TABLE #EmployeeAttendance (
		EmpId INT
		,AttendanceDate DATE
		,MissingType TINYINT
		);

	/* Get have not request missing check in check out*/
	INSERT INTO #EmployeeAttendance (
		EmpId
		,AttendanceDate
		,MissingType
		)
	SELECT a.EmpId
		,AttendanceDate
		,1
	FROM [dbo].EmployeeAttendance AS a WITH (NOLOCK)
	INNER JOIN [dbo].EmployeeCalendar AS C WITH (NOLOCK) ON c.EmpId = a.EmpId
		AND c.WorkDate = AttendanceDate
	WHERE AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND a.EmpId = @EmployeeId
		AND InFixed IS NULL
		AND IsOffDay = 0
		AND IsHoliday = 0
		AND (
			IsLeaveDay = 0
			OR (
				IsLeaveDay = 1
				AND a.LeaveDay = 0.5
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM [dbo].WorkingTimeRequest AS r WITH (NOLOCK)
			WHERE r.EmpId = a.EmpId
				AND r.EmpId = @EmployeeId
				AND RequestType = 1
				AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
			)
		AND NOT EXISTS (
			SELECT 1
			FROM [dbo].WorkingTimeRequest AS r WITH (NOLOCK)
			WHERE r.EmpId = a.EmpId
				AND r.EmpId = @EmployeeId
				AND RequestType = 6
				AND r.TotalDays >= 1
				AND AttendanceDate BETWEEN r.FromDate
					AND r.ToDate
				AND r.[Status] <> 3
			);

	INSERT INTO #EmployeeAttendance (
		EmpId
		,AttendanceDate
		,MissingType
		)
	SELECT a.EmpId
		,AttendanceDate
		,2
	FROM [dbo].EmployeeAttendance AS a WITH (NOLOCK)
	INNER JOIN [dbo].EmployeeCalendar AS C WITH (NOLOCK) ON c.EmpId = a.EmpId
		AND c.WorkDate = AttendanceDate
	WHERE AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND a.EmpId = @EmployeeId
		AND OutFixed IS NULL
		AND IsOffDay = 0
		AND IsHoliday = 0
		AND (
			IsLeaveDay = 0
			OR (
				IsLeaveDay = 1
				AND a.LeaveDay = 0.5
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM [dbo].WorkingTimeRequest AS r WITH (NOLOCK)
			WHERE r.EmpId = a.EmpId
				AND r.EmpId = @EmployeeId
				AND RequestType = 2
				AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
			)
		AND NOT EXISTS (
			SELECT 1
			FROM [dbo].WorkingTimeRequest AS r WITH (NOLOCK)
			WHERE r.EmpId = a.EmpId
				AND r.EmpId = @EmployeeId
				AND RequestType = 6
				AND r.TotalDays >= 1
				AND AttendanceDate BETWEEN r.FromDate
					AND r.ToDate
				AND r.[Status] <> 3
			);

	/* End Get have not request missing check in check out*/
	SELECT at.[Month]
		,at.[Year]
		,at.MissingCheckInCheckOut
		,at.LateIn
		,at.EarlyOut
		,at.UnpaidLeave
		,at.UnplanAnnualLeave
		,at.AverageWorkingHours
		,ot.Overtime
	FROM
		(
			SELECT MONTH(et.AttendanceDate) AS [Month]
				,YEAR(et.AttendanceDate) AS [Year]
				,SUM(CASE 
						WHEN req.EmpId IS NOT NULL
							THEN 1
						ELSE 0
						END) + SUM(CASE 
						WHEN tpl.AttendanceDate IS NOT NULL
							THEN 1
						ELSE 0
						END) AS MissingCheckInCheckOut
				,SUM(CASE 
						WHEN et.LateIn > 0
							THEN 1
						ELSE 0
						END) AS LateIn
				,SUM(CASE 
						WHEN et.EarlyOut > 0
							THEN 1
						ELSE 0
						END) AS EarlyOut
				,SUM(CASE 
						WHEN (
								et.LeaveTypeId = 26
								AND ISNULL(et.LeaveTypeId2, 0) <> 26
								)
							OR (
								et.LeaveTypeId2 = 26
								AND ISNULL(et.LeaveTypeId, 0) <> 26
								)
							THEN 1
						WHEN (
								et.LeaveTypeId = 26
								AND et.LeaveTypeId2 = 26
								)
							THEN 2
						ELSE 0
						END) AS UnpaidLeave
				,SUM(CASE 
						WHEN (
								et.LeaveTypeId = 30
								AND ISNULL(et.LeaveTypeId2, 0) <> 30
								)
							OR (
								et.LeaveTypeId2 = 30
								AND ISNULL(et.LeaveTypeId, 0) <> 30
								)
							THEN 1
						WHEN (
								et.LeaveTypeId = 30
								AND et.LeaveTypeId2 = 30
								)
							THEN 2
						ELSE 0
						END) AS UnplanAnnualLeave
				,(
					CAST(ROUND(SUM(et.TotalWorkHours) / SUM(CASE 
									WHEN et.TotalWorkHours IS NOT NULL
										THEN (
												CASE 
													WHEN et.LeaveDay < 1
														THEN et.LeaveDay
													ELSE 1
													END
												)
									ELSE 0
									END), 2) AS NUMERIC(36, 2))
					) AverageWorkingHours
			FROM [dbo].[EmployeeAttendance] AS et WITH (NOLOCK)
			LEFT JOIN [dbo].[EmployeeCalendar] AS ec WITH (NOLOCK) ON et.EmpId = ec.EmpId
				AND ec.WorkDate = et.AttendanceDate
			LEFT JOIN [dbo].[WorkingTimeRequest] AS req WITH (NOLOCK) ON req.EmpId = ec.EmpId
				AND req.RequestType IN (
					1
					,2
					)
				AND CONVERT(DATE, req.FromDate) = et.AttendanceDate
			LEFT JOIN #EmployeeAttendance AS tpl WITH (NOLOCK) ON tpl.AttendanceDate = et.AttendanceDate
			WHERE et.EmpId = @EmployeeId
				AND et.AttendanceDate BETWEEN @FromDate
					AND @ToDate
			GROUP BY MONTH(et.AttendanceDate)
				,YEAR(et.AttendanceDate)
		) AS at
	LEFT JOIN
		(
			SELECT MONTH(ot.[Date] ) AS [Month]
				,YEAR(ot.[Date] ) AS [Year]
				,SUM(ot.[Hour]) AS Overtime
			FROM dbo.EmployeeOvertime AS ot WITH (NOLOCK)
			WHERE ot.EmployeeId = @EmployeeId
			AND ot.[Date] BETWEEN @FromDate AND @ToDate
			GROUP BY MONTH(ot.[Date] ), YEAR(ot.[Date] )
		) AS ot ON at.[Month] = ot.[Month] AND at.[Year] = ot.[Year]

	--Get previous PA Grade
	DECLARE @PreviuosPeriodId INT
			,@SelfProcessId TINYINT = 2
			,@EvaluatorProcessId TINYINT = 4
			,@ReviewerProcessId TINYINT = 5
	
	SELECT TOP 1 @PreviuosPeriodId = pe.PeriodId
	FROM dbo.Period AS pe WITH (NOLOCK)
	WHERE pe.StartDate < (
			SELECT p.StartDate
			FROM dbo.Period AS p WITH (NOLOCK)
			WHERE p.PeriodId = @CurrentPeriodId
			)
	ORDER BY pe.StartDate

	SELECT pa.GradeId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
	INNER JOIN dbo.PAProcess AS pr WITH (NOLOCK) ON pa.PARecordId = pr.PARecordId
	WHERE pa.PeriodId = @PreviuosPeriodId
	AND pa.EmployeeId = @EmployeeId
	AND pr.EvaluatorId = @ActorId
	AND pr.ProcessId IN (@SelfProcessId, @ReviewerProcessId, @EvaluatorProcessId)
END