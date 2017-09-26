



CREATE PROCEDURE [dbo].[HRMSummary_GetOnProbation] @EmployeeId INT
AS

/*
Created: 20160804@Razor.nguyen
Task: PA Form
DB: NexGoal
Original:

Revisions:
Param's Explanation:

*/

     BEGIN
         SELECT T.[Month],
                T.[Year],
                T.NumberEarlyOut,
                T.NumberLateIn,
                T.NumberMissingCheckInOut,
                T.NumberUnpaidLeave,
                T.NumberUnplannedAnnualLeave,
                T.AverageWorkingHours,
                [to].Listening AS ListeningScore,
                [to].Reading AS ReadingScore,
                [to].ExamDate As UpdatedDateTOEIC,
                ISNULL(ot.Times, 0) AS NumberOvertime,
                @EmployeeId AS EmployeeId
         FROM
         (
             SELECT MONTH(hrm.AttendanceDate) AS [Month],
                    YEAR(hrm.AttendanceDate) AS [Year],
                    SUM(hrm.IsMissingCheckInOut) AS NumberMissingCheckInOut,
                    SUM(hrm.IsLateIn) AS NumberLateIn,
                    SUM(hrm.IsEarlyOut) AS NumberEarlyOut,
                    SUM(hrm.IsUnpaidLeave) AS NumberUnpaidLeave,
                    SUM(hrm.IsUnplannedAnnualLeave) AS NumberUnplannedAnnualLeave,
                    SUM(hrm.TotalWorkHours)/SUM(ActualWorkDays) AS AverageWorkingHours
             FROM dbo.HRMSummary AS hrm WITH(NOLOCK)
             INNER JOIN dbo.Employee as e with(nolocK) ON e.EmployeeId = hrm.EmployeeId
             WHERE hrm.AttendanceDate >= e.JoinedDate AND hrm.AttendanceDate <= DATEADD(Month,1,e.JoinedDate)
                   AND hrm.EmployeeId = @EmployeeId
             GROUP BY MONTH(hrm.AttendanceDate),
                      YEAR(hrm.AttendanceDate)
         ) AS T
         LEFT JOIN dbo.TOEIC AS [to] ON [to].EmployeeId = @EmployeeId
         LEFT JOIN
         (
             SELECT MONTH(ot.[Date]) AS [month],
                    YEAR(ot.[Date]) AS [year],
                    COUNT(*) AS Times
             FROM dbo.Overtime AS ot WITH (NOLOCK)
             INNER JOIN dbo.Employee as e with(nolocK) ON e.EmployeeId = ot.EmployeeId
             WHERE ot.EmployeeId = @EmployeeId
                   AND [Date] >= e.JoinedDate AND [Date] <= DATEADD(Month,1,e.JoinedDate)
             GROUP BY MONTH(ot.[Date]),
                      YEAR(ot.[Date])
         ) AS ot ON ot.[month] = T.[month]
                    AND ot.[year] = T.[year];
     END;