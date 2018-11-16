CREATE PROCEDURE [dbo].[Employee_GetHRMSummary] @EmployeeId INT
AS
/*
Created: 20180926@Oliver.Dang
Task: HRM get summary.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@toeic INT
		,@isPassedToeic BIT
		,@examDate DATETIME
		,@incident INT
		,@missingCheckInCheckOut INT

	SELECT @FromDate = DisciplineStartDate
		,@ToDate = DisciplineEndDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND DisciplineStartDate <= GETDATE()
	ORDER BY DisciplineStartDate DESC

	CREATE TABLE #EmployeeAttendance (
		EmpId INT
		,AttendanceDate DATETIME
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
	FROM [dbo].EmployeeAttendance a WITH (NOLOCK)
	INNER JOIN [dbo].EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
		AND c.WorkDate = AttendanceDate
	WHERE attendancedate BETWEEN @FromDate
			AND @ToDate
		AND a.EmpId = @EmployeeId
		AND InFixed IS NULL
		AND isOffday = 0
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
			FROM [dbo].WorkingTimeRequest r WITH (NOLOCK)
			WHERE r.empid = a.empid
				AND r.EmpId = @EmployeeId
				AND requesttype = 1
				AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
			)
		AND NOT EXISTS (
			SELECT 1
			FROM [dbo].WorkingTimeRequest r WITH (NOLOCK)
			WHERE r.empid = a.empid
				AND r.EmpId = @EmployeeId
				AND requesttype = 6
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
	FROM [dbo].EmployeeAttendance a WITH (NOLOCK)
	INNER JOIN [dbo].EmployeeCalendar C WITH (NOLOCK) ON c.empID = a.EmpID
		AND c.WorkDate = AttendanceDate
	WHERE attendancedate BETWEEN @FromDate
			AND @ToDate
		AND a.EmpId = @EmployeeId
		AND OutFixed IS NULL
		AND isOffday = 0
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
			FROM [dbo].WorkingTimeRequest r WITH (NOLOCK)
			WHERE r.empid = a.empid
				AND r.EmpId = @EmployeeId
				AND requesttype = 2
				AND DATEDIFF(d, r.FromDate, AttendanceDate) = 0
			)
		AND NOT EXISTS (
			SELECT 1
			FROM [dbo].WorkingTimeRequest r WITH (NOLOCK)
			WHERE r.empid = a.empid
				AND r.EmpId = @EmployeeId
				AND requesttype = 6
				AND r.TotalDays >= 1
				AND AttendanceDate BETWEEN r.FromDate
					AND r.ToDate
				AND r.[Status] <> 3
			);

	/* End Get have not request missing check in check out*/
	SELECT @toeic = (Listening + Reading)
		,@examDate = LRExamDate
		,@isPassedToeic = (
			CASE 
				WHEN (ISNULL(Listening, 0) + ISNULL(Reading, 0)) >= ISNULL(MinimumScore, 0)
					OR (ISNULL(Listening, 0) + ISNULL(Reading, 0)) >= 900
					THEN 1
				ELSE 0
				END
			)
	FROM [dbo].[TOEIC] WITH (NOLOCK)
	WHERE EmpId = @EmployeeId

	SET @incident = (
			SELECT COUNT(1)
			FROM dbo.Discipline WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
				AND CreateDate BETWEEN @FromDate
					AND @ToDate
			)
	SET @missingCheckInCheckOut = (
			SELECT COUNT(1)
			FROM [dbo].[WorkingTimeRequest] WITH (NOLOCK)
			WHERE (
					RequestType = 1
					OR RequestType = 2
					)
				AND EmpId = @EmployeeId
				AND FromDate BETWEEN @FromDate
					AND @ToDate
			) + (
			SELECT COUNT(*)
			FROM #EmployeeAttendance AS ea WITH (NOLOCK)
			)

	DROP TABLE #EmployeeAttendance

	SELECT @toeic AS ToeicScore
		,@examDate AS ToeicExamDate
		,@isPassedToeic AS IsPassedToeic
		,@incident AS Incident
		,@missingCheckInCheckOut AS MissingCheckInCheckOut
		,SUM(CASE 
				WHEN (
						LateIn > 0
						AND EarlyOut = 0
						)
					OR (
						LateIn = 0
						AND EarlyOut > 0
						)
					THEN 1
				WHEN (
						LateIn > 0
						AND EarlyOut > 0
						)
					THEN 2
				ELSE 0
				END) AS TotalLateInEarlyOut
		,SUM(CASE 
				WHEN (
						LeaveTypeId = 26
						AND ISNULL(LeaveTypeId2, 0) <> 26
						)
					OR (
						LeaveTypeId2 = 26
						AND ISNULL(LeaveTypeId, 0) <> 26
						)
					THEN 1
				WHEN (
						LeaveTypeId = 26
						AND LeaveTypeId2 = 26
						)
					THEN 2
				ELSE 0
				END) AS UnpaidLeave
		,SUM(CASE 
				WHEN (
						LeaveTypeId = 30
						AND ISNULL(LeaveTypeId2, 0) <> 30
						)
					OR (
						LeaveTypeId2 = 30
						AND ISNULL(LeaveTypeId, 0) <> 30
						)
					THEN 1
				WHEN (
						LeaveTypeId = 30
						AND LeaveTypeId2 = 30
						)
					THEN 2
				ELSE 0
				END) AS UnplanAnnualLeave
		,(
			SUM(CASE 
					WHEN et.Absence IS NOT NULL
						AND et.Absence = 0
						AND c.IsLeaveDay = 0
						THEN 1
					WHEN et.Absence IS NOT NULL
						AND et.Absence = 0
						AND c.IsLeaveDay = 1
						AND c.LeaveDay < 1
						THEN 0.5
					ELSE 0
					END) / (DATEDIFF(month, @FromDate, @ToDate) + 1)
			) AS AverageWorkingDays
		,(
			SELECT SUM(st.[Hours])
			FROM [dbo].[ProjectSpentTime] AS st WITH (NOLOCK)
			WHERE st.EmployeeId = @EmployeeId
				AND st.SpentOn BETWEEN @FromDate
					AND @ToDate
			) AS TotalSpentTime
		,SUM(et.TotalWorkHours) AS TotalWorkHours
	FROM [dbo].[EmployeeAttendance] AS et WITH (NOLOCK)
	INNER JOIN [dbo].[EmployeeCalendar] AS c WITH (NOLOCK) ON et.AttendanceDate = c.WorkDate
		AND et.EmpId = c.EmpId
	WHERE AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND et.EmpId = @EmployeeId
END