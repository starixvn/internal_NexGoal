
CREATE PROCEDURE [dbo].[PARecord_GetGroupByActor] @ActorId INT
	,@PeriodId INT
AS
/*
Created: 20180921@Tim.Trinh
Task: Get group by actor.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @SelfProcessId TINYINT = 2

	SELECT DISTINCT gr.GroupId
		,gr.[Name]
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.EvaluatorId = @ActorId
		AND process.ProcessId > @SelfProcessId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
END
