
/****** Object:  Stored Procedure dbo.EPA_GetKPIGroup    Script Date: 7/21/2016 2:45:44 PM ******/

CREATE PROCEDURE [dbo].[EPA_GetKPIGroupScore] @EPAIds    INTID READONLY,
                                             @ProcessId INT
AS

/*
Created: 20150827@Tim.Trinh
Task: Get KPI Group by list id.
DB: SPM
Original: 
 
Param's Explanation:

*/

     BEGIN
         SET NOCOUNT ON;
         SELECT DISTINCT
                g.EPAId,
                g.Score AS ScoreEncode,
                g.ProcessId,
                g.EvaluatorId,
                g.KPIGroupId
         FROM dbo.EPAGroupDetail AS g
              INNER JOIN @EPAIds AS epa ON epa.Id = g.EPAId
         WHERE g.ProcessId = @ProcessId;
     END;