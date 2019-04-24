CREATE PROCEDURE [dbo].[Employee_GetHRMSummary] @EmployeeId INT
AS
/*
Created: 20180926@Oliver.Dang
Task: HRM get summary.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATE
		,@ToDate DATE
		,@toeic INT
		,@ToeicPolicy SMALLINT
		,@examDate DATE
		,@incident INT
		,@missingCheckInCheckOut INT
		,@Today DATE = GETDATE()

	SELECT TOP 1 @FromDate = DisciplineStartDate
		,@ToDate = DisciplineEndDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND DisciplineStartDate <= @Today
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
	FROM [dbo].EmployeeAttendance AS a WITH (NOLOCK)
	INNER JOIN [dbo].EmployeeCalendar AS C WITH (NOLOCK) ON c.EmpId = a.EmpId
		AND c.WorkDate = AttendanceDate
	WHERE AttendanceDate BETWEEN @FromDate
			AND @Today
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
			AND DATEADD(day, - 1, @Today)
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
	SELECT @toeic = (Listening + Reading)
		,@examDate = LRExamDate
		,@ToeicPolicy = policy.Score
	FROM [dbo].[TOEIC] AS toeic WITH (NOLOCK)
	LEFT JOIN [dbo].[ToeicPolicy] AS policy WITH (NOLOCK) ON toeic.EmpId = policy.EmployeeId
	WHERE EmpId = @EmployeeId

	SET @incident = (
			SELECT COUNT(1)
			FROM dbo.Discipline WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
				AND EffectiveDate BETWEEN @FromDate
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
		,@ToeicPolicy AS ToeicPolicy
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
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId = 26
						AND ISNULL(et.LeaveTypeId2, 0) <> 26
						)
					OR (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId2 = 26
						AND ISNULL(et.LeaveTypeId, 0) <> 26
						)
					THEN 1
				WHEN (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId = 26
						AND et.LeaveTypeId2 = 26
						)
					THEN 2
				ELSE 0
				END) AS UnpaidLeave
		,SUM(CASE 
				WHEN (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId = 28
						AND ISNULL(et.LeaveTypeId2, 0) <> 28
						)
					OR (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId2 = 28
						AND ISNULL(et.LeaveTypeId, 0) <> 28
						)
					THEN 1
				WHEN (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId = 28
						AND et.LeaveTypeId2 = 28
						)
					THEN 2
				ELSE 0
				END) AS SickLeave
		,SUM(CASE 
				WHEN (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId = 30
						AND ISNULL(et.LeaveTypeId2, 0) <> 30
						)
					OR (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId2 = 30
						AND ISNULL(LeaveTypeId, 0) <> 30
						)
					THEN 1
				WHEN (
						c.IsHoliday <> 1
						AND c.IsOffDay <> 1
						AND et.LeaveTypeId = 30
						AND et.LeaveTypeId2 = 30
						)
					THEN 2
				ELSE 0
				END) AS UnplannedAnnualLeave
		,(
			CAST(ROUND((
						SELECT SUM(CASE 
									WHEN calendar.IsHoliday <> 1
										AND calendar.IsOffDay <> 1
										AND calendar.IsLeaveDay <> 1
										THEN 1
									WHEN calendar.IsLeaveDay = 1
										THEN calendar.LeaveDay
									ELSE 0
									END)
						FROM [dbo].[EmployeeCalendar] AS calendar WITH (NOLOCK)
						WHERE EmpId = @EmployeeId
							AND WorkDate BETWEEN @FromDate
								AND @ToDate
						) / (DATEDIFF(month, @FromDate, @ToDate) + 1), 2) AS NUMERIC(36, 2))
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
