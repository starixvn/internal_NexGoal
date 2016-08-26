
CREATE PROCEDURE [dbo].[EPA_GetMember] @ActorUserId INT,
                                       @ProcessId   INT,
                                       @GroupIds    VARCHAR(MAX) = NULL,
                                       @PeriodIds   VARCHAR(MAX) = NULL
AS

/*
Created: 20150818@Tim.Trinh
Task: Get Member by Evaluator or Reviewer or Management.
DB: SMSEPA
Original:

Revisions:  
Param's Explanation:

*/

     BEGIN
         SET NOCOUNT ON;
         DECLARE @Today DATE, @Periods INTID, @Groups INTID;
         SET @Today = CAST(GETDATE() AS DATE);
         IF ISNULL(@PeriodIds, '') <> ''
             BEGIN
                 INSERT INTO @Periods(Id)
                        SELECT DISTINCT
                               fnc_strSplit.val
                        FROM dbo.fnc_strSplit(@PeriodIds, ',');
             END;
         IF ISNULL(@GroupIds, '') <> ''
             BEGIN
                 INSERT INTO @Groups(Id)
                        SELECT DISTINCT
                               fnc_strSplit.val
                        FROM dbo.fnc_strSplit(@GroupIds, ',');
             END;
         SELECT DISTINCT
                emp.EmployeeId,
                emp.Code,
                emp.NickName,
                emp.FullName,
                emp.[Enabled],
                emp.Email
         FROM dbo.EPA AS epa WITH (NOLOCK)
              INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
                                                               AND pc.EvaluatorId = @ActorUserId
                                                               AND pc.ProcessId = @ProcessId
              INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
                                                              AND emp.[Enabled] = 1
                                                              AND emp.IsDeleted = 0
              INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON emp.GroupId = g.GroupId
              INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
                                                               AND period.IsActive = 1
              INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
         WHERE pc.StartDate <= @Today
              AND epa.[Status] <> 2 --reject
              AND (@GroupIds IS NULL
                   OR g.GroupId IN
                  (
                      SELECT Id
                      FROM @Groups
                  ))
              AND (@PeriodIds IS NULL
                   OR period.PeriodId IN
                  (
                      SELECT Id
                      FROM @Periods
                  ))
         ORDER BY emp.NickName,
                  emp.Code;
     END;
