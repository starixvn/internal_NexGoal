

CREATE PROCEDURE [dbo].[Period_GetInprogressSemiPeriod] @EmployeeId INT
AS

/*
Created: 20160804@Razor.nguyen
Task: PA Form
DB: NexGoal
Original:

Revisions:
Param's Explanation:

*/

     DECLARE @EPAStatusIsInprogress TINYINT, @TargetTypeIsSemi INT;
     SET @EPAStatusIsInprogress = 0;
     SET @TargetTypeIsSemi = 3;
     SELECT p.PeriodId,
            p.TargetTypeId,
            p.StartDate,
            p.EndDate,
            p.AttendanceStartDate,
            p.AttendanceEndDate,
            p.IsActive
     FROM dbo.Period p WITH (NOLOCK)
          INNER JOIN dbo.EPA epa WITH (NOLOCK) ON epa.PeriodId = p.PeriodId
          INNER JOIN
     (
         SELECT epa.EpaId,
                MAX(pc.endDate) AS endDate
         FROM dbo.EPAProcess pc WITH (NOLOCK)
              INNER JOIN dbo.Epa AS epa ON epa.EpaId = pc.EpaId
         WHERE epa.EmployeeId = @EmployeeId
         GROUP BY epa.EpaId
     ) AS pc ON pc.EpaId = epa.EpaId
     WHERE p.IsActive = 1
           AND epa.EmployeeId = @EmployeeId
           AND epa.Status = @EPAStatusIsInprogress
           AND p.TargetTypeId = @TargetTypeIsSemi
           AND (p.EndDate > GETDATE()
                OR pc.enddate > GETDATE());