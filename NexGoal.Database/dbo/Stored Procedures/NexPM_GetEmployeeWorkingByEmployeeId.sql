
CREATE PROCEDURE [dbo].[NexPM_GetEmployeeWorkingByEmployeeId] @EmployeeId INT,
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
         SELECT pm.ProjectName,
                pm.ProjectId,
                pm.StartDate,
                pm.EndDate,
                pm.Roles,
                pm.Score AS EncryptedScore,
                pm.Grade AS EncryptedGrade,
                pm.Comment,
                pm.EmployeeId,
                ISNULL(rm.Hours, 0) AS WorkingHour,
                pm.projectManager AS ProjectManagerId,
                m.NickName
         FROM dbo.NexPM AS pm WITH (NOLOCK)
              INNER JOIN dbo.Employee AS m ON m.EmployeeId = pm.projectManager
              LEFT JOIN
         (
             SELECT ProjectName,
                    SUM(hours) AS Hours
             FROM dbo.Redmine AS rm WITH (NOlock)
                  INNER JOIN dbo.Employee AS e WITH (nolock) ON e.NickName = rm.NickName
             WHERE e.EmployeeId = @EmployeeId
                   AND rm.SpentOn BETWEEN @StartDate AND @EndDate
             GROUP BY ProjectName
         ) AS rm ON rm.ProjectName = pm.ProjectName
         WHERE pm.EndDate >= @StartDate
               AND (pm.EndDate <= @EndDate
                    OR pm.EndDate IS NULL)
               AND pm.EmployeeId = @EmployeeId;
     END;