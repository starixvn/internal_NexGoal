

CREATE PROCEDURE [dbo].[EPA_GetGroup] @ActorUserId INT,
                                      @ProcessId   INT,
                                      @PeriodIds   VARCHAR(MAX) = NULL
AS

/*
Created: 20151109@Tim.Trinh
Task: Get Group by Evaluator or Reviewer or Management.
DB: SMSEPA
Original: 
 
Param's Explanation:

*/

     BEGIN
         SET NOCOUNT ON;
         DECLARE @Today DATE, @Periods INTID;
         SET @Today = CAST(GETDATE() AS DATE);
         IF ISNULL(@PeriodIds, '') <> ''
             BEGIN
                 INSERT INTO @Periods(Id)
                        SELECT DISTINCT
                               fnc_strSplit.val
                        FROM dbo.fnc_strSplit(@PeriodIds, ',');
             END;
         SELECT DISTINCT
                gr.[GroupId],
                gr.[Name] AS GroupName,
                gr.[Code] AS GroupCode
         FROM dbo.EPA AS epa WITH (NOLOCK)
              INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
                                                               AND pc.EvaluatorId = @ActorUserId
                                                               AND pc.ProcessId = @ProcessId
              INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
                                                               AND period.IsActive = 1
              INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
                                                              AND emp.[Enabled] = 1
                                                              AND emp.IsDeleted = 0
              LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
                                                           AND gr.IsDeleted = 0
         WHERE CAST(pc.StartDate AS DATE) <= @Today
         AND epa.[Status] <> 2 
         AND @PeriodIds IS NULL
               OR period.PeriodId IN
         (
             SELECT Id
             FROM @Periods
         );
     END;
