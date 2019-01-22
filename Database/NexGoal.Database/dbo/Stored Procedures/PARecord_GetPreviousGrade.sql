CREATE PROCEDURE [dbo].[PARecord_GetPreviousPAGrade] @ActorId INT
	,@EmployeeId INT
	,@CurrentPeriodId INT
AS
/*
Created: 20190121@Oliver.Dang
Task: Get Previous PA Grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @PreviuosPeriodId INT
			,@SelfProcessId TINYINT = 2
			,@EvaluatorProcessId TINYINT = 4
			,@ReviewerProcessId TINYINT = 5
	
	SELECT TOP 1 @PreviuosPeriodId = pe.PeriodId
	FROM dbo.Period AS pe WITH (NOLOCK)
	WHERE pe.StartDate < (
			SELECT p.StartDate
			FROM dbo.Period AS p WITH (NOLOCK)
			WHERE p.PeriodId = @CurrentPeriodId
			)
	ORDER BY pe.StartDate

	SELECT pa.GradeId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
	INNER JOIN dbo.PAProcess AS pr WITH (NOLOCK) ON pa.PARecordId = pr.PARecordId
	WHERE pa.PeriodId = @PreviuosPeriodId
	AND pa.EmployeeId = @EmployeeId
	AND pr.EvaluatorId = @ActorId
	AND pr.ProcessId IN (@SelfProcessId, @ReviewerProcessId, @EvaluatorProcessId)
END
