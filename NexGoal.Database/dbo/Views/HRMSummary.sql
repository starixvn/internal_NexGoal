


CREATE VIEW [dbo].[HRMSummary]
AS
     SELECT T.EmpId AS EmployeeId,
            T.AttendanceDate,
            T.InFixed,
            T.OutFixed,
            T.TotalWorkHours,
            T.ActualWorkDays,
            T.IsUnpaidLeave,
            T.IsUnplannedAnnualLeave,
            T.IsLateIn,
            T.IsEarlyOut,
            SUM(T.IsMissingCheckIn)+SUM(T.IsMissingCheckOut) AS IsMissingCheckInOut
     FROM
     (
         SELECT a.EmpId,
                AttendanceDate,
                a.InFixed,
                a.OutFixed,
                a.LateIn,
                a.EarlyOut,
                TotalWorkHours,SUM(CASE 
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 0 THEN 1
						WHEN Absence IS NOT NULL
							AND Absence = 0
							AND ISNULL(a.TotalWorkHours, 0) <> 0
							AND IsLeaveDay = 1
							AND c.LeaveDay < 1
							AND c.LeaveTypeId <> 9 THEN 0.5
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND c.LeaveTypeId = 9
							AND (
								c.LeaveTypeId2 IS NULL
								OR c.LeaveTypeId2 = 9
								) THEN 1
						WHEN IsLeaveDay = 1
							AND IsOffDay = 0
							AND (
								c.LeaveTypeId = 9
								OR ISNULL(c.LeaveTypeId2, 0) = 9
								)
							AND (
								c.LeaveTypeId <> 9
								OR ISNULL(c.LeaveTypeId2, 0) <> 9
								) THEN 0.5
						ELSE 0
						END)
			  AS [ActualWorkDays]
                ,CASE
                    WHEN a.LeaveTypeId = 26
                    THEN 1
                    ELSE 0
                END IsUnpaidLeave,
                CASE
                    WHEN a.LeaveTypeId = 30
                    THEN 1
                    ELSE 0
                END IsUnplannedAnnualLeave,
                CASE
                    WHEN e.Id NOT IN(88, 89, 123, 134, 153)
                AND ISNULL(Absence, 0) = 0
                AND ((DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
                      AND ISNULL(a.LateIn, 0) > 15)
                     OR (DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                         AND ISNULL(a.LateIn, 0) > 0))
                    THEN 1
                    ELSE 0
                END IsLateIn,
                CASE
                    WHEN e.Id NOT IN(88, 89, 123, 134, 153)
         AND ISNULL(Absence, 0) = 0
         AND ((DATEDIFF(D, a.AttendanceDate, '2015-08-01') > 0
               AND ISNULL(a.EarlyOut, 0) > 15)
              OR (DATEDIFF(D, '2015-08-01', a.AttendanceDate) >= 0
                  AND ISNULL(a.EarlyOut, 0) > 0))
                    THEN 1
                    ELSE 0
                END IsEarlyOut,
                CASE
                    WHEN e.Id NOT IN(88, 89, 123, 134, 153)
     AND r.RequestType = 1
                    THEN 1
                    ELSE 0
                END IsMissingCheckIn,
                CASE
                    WHEN e.Id NOT IN(88, 89, 123, 134, 153)
     AND r.RequestType = 2
                    THEN 1
                    ELSE 0
                END IsMissingCheckOut
         FROM NexHRM.dbo.EmployeeAttendance a WITH (NOLOCK)
              JOIN NexHRM.dbo.EmployeeCalendar c WITH (NOLOCK) ON c.EmpId = a.EmpId
                                                                  AND c.Workdate = a.AttendanceDate
              INNER JOIN NexHRM.dbo.Employee e WITH (NOLOCK) ON a.Empid = e.Id
              LEFT JOIN NexHRM.[dbo].[WorkingTimeRequest] r WITH (Nolock) ON r.[EmpId] = e.Id
                                                                             AND CONVERT( DATE, r.FromDate) = CONVERT(DATE, a.AttendanceDate)
                                                                             AND r.RequestType IN(1, 2)
              AND r.[status] = 2
              LEFT JOIN NexHRM.dbo.Shift s WITH (NOLOCK) ON s.Id = a.ShiftId
         WHERE e.IsSpecialUser = 0
               AND ((e.isDeleted IS NULL
                     OR e.isDeleted = 0))
         GROUP BY a.EmpId,
         e.Id,Absence,
         r.RequestType,
                  AttendanceDate,
                  a.InFixed,
                  a.OutFixed,
                  a.LateIn,
                  a.EarlyOut,
                  a.LeaveTypeId,
                  TotalWorkHours
     ) T
     group by T.EmpId,
            T.AttendanceDate,
            T.InFixed,
            T.OutFixed,
            T.TotalWorkHours,
            T.ActualWorkDays,
            T.IsUnpaidLeave,
            T.IsUnplannedAnnualLeave,
            T.IsLateIn,
            T.IsEarlyOut