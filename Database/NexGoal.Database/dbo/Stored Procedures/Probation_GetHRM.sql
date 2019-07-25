
CREATE PROCEDURE [dbo].[Probation_GetHRM]
	@EmployeeId INT
AS
/*
Created: 20190613@Elton.Ton
Task: get hrm info
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @FromDate DATE, @ToDate DATE

	SELECT @FromDate = JoinedDate
	FROM dbo.Employee
	WHERE EmployeeId = @EmployeeId

	SET @ToDate = DATEADD(MONTH, 2, @FromDate)

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

	SELECT t.EmpId AS EmployeeId
		,(t.Listening + t.Reading) AS 'ToeicScore'
		,t.LRExamDate AS 'ExamDate'
	FROM dbo.TOEIC t 
	WHERE t.EmpId = @EmployeeId
END