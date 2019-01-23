CREATE PROCEDURE [dbo].[Report_GetHRM] @EmployeeId INT
	,@PeriodId INT
AS
/*
Created: 20190122@Oliver.Dang
Task: Get HRM info in PA Report.
DB: bodb_NexGoal
Original:

*/
BEGIN
	DECLARE @FromDate DATETIME
		,@ToDate DATETIME

	SELECT @FromDate = p.DisciplineStartDate
		,@ToDate = p.DisciplineEndDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

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
								THEN 1
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
END
