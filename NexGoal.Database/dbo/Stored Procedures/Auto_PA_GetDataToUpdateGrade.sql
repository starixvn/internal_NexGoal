
CREATE PROCEDURE [dbo].[Auto_PA_GetDataToUpdateGrade]
AS
/*
Created: 20181113@Tim.Trinh
Task: get data to update grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Tomorrow DATE = DATEADD(DAY, 1, GETDATE())
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT pa.PARecordId
		,pa.EmployeeId
		,record.GradeId
		,toeic.Score AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PAProcess AS evaluator WITH (NOLOCK) ON pa.PARecordId = evaluator.PARecordId
		AND evaluator.ProcessId = @EvaluatorProcessId
		INNER JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON evaluator.PerformanceRecordId = record.PerformanceRecordId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE pa.StatusId = @PAInProgress
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS reviewer WITH (NOLOCK)
			WHERE pa.PARecordId = reviewer.PARecordId
				AND reviewer.ProcessId = @ReviewerProcessId
				AND reviewer.StartDate = @Tomorrow
			)
END