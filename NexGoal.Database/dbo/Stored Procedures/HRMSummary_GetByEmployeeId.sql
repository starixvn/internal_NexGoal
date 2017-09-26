



                                                   
CREATE PROCEDURE [dbo].[HRMSummary_GetByEmployeeId] @EmployeeId INT,
                                                   @StartDate  DATETIME,
                                                   @EndDate    DATETIME
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
             SELECT MONTH(AttendanceDate) AS [Month],
                    YEAR(AttendanceDate) AS [Year],
                    SUM(IsMissingCheckInOut) AS NumberMissingCheckInOut,
                    SUM(IsLateIn) AS NumberLateIn,
                    SUM(IsEarlyOut) AS NumberEarlyOut,
                    SUM(IsUnpaidLeave) AS NumberUnpaidLeave,
                    SUM(IsUnplannedAnnualLeave) AS NumberUnplannedAnnualLeave,
                    SUM(TotalWorkHours)/SUM(ActualWorkDays) AS AverageWorkingHours
             FROM dbo.HRMSummary WITH(NOLOCK)
             WHERE AttendanceDate BETWEEN @StartDate AND @EndDate
                   AND EmployeeId = @EmployeeId
             GROUP BY MONTH(AttendanceDate),
                      YEAR(AttendanceDate)
         ) AS T
         LEFT JOIN dbo.TOEIC AS [to] ON [to].EmployeeId = @EmployeeId
         LEFT JOIN
         (
             SELECT MONTH(Date) AS [month],
                    YEAR(Date) AS [year],
                    COUNT(*) AS Times
             FROM dbo.Overtime AS ot WITH (NOLOCK)
             WHERE EmployeeId = @EmployeeId
                   AND Date BETWEEN @StartDate AND @EndDate
             GROUP BY MONTH(Date),
                      YEAR(Date)
         ) AS ot ON ot.[month] = T.[month]
                    AND ot.[year] = T.[year];
     END;