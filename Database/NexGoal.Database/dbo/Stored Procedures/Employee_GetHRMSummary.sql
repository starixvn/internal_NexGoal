
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
			)

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
						LeaveTypeId = 28
						AND ISNULL(LeaveTypeId2, 0) <> 28
						)
					OR (
						LeaveTypeId2 = 28
						AND ISNULL(LeaveTypeId, 0) <> 28
						)
					THEN 1
				WHEN (
						LeaveTypeId = 28
						AND LeaveTypeId2 = 28
						)
					THEN 2
				ELSE 0
				END) AS SickLeave
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
						FROM [NexHRM].[dbo].[EmployeeCalendar] AS calendar WITH (NOLOCK)
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
