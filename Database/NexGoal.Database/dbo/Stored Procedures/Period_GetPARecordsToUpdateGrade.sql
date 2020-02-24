
CREATE PROCEDURE [dbo].[Period_GetPARecordsToUpdateGrade] @PARecordIds VARCHAR(MAX)
AS
/*
Created: 20181126@Elton.ton
Task: get data to update grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EvaluatorProcessId TINYINT = 4

	SELECT pa.PARecordId
		,pa.EmployeeId
		,evaluator.GradeId
		,toeic.Score AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.PAProcess AS evaluator WITH (NOLOCK) ON pa.PARecordId = evaluator.PARecordId
		AND evaluator.ProcessId = @EvaluatorProcessId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE period.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM dbo.func_SplitString(@PARecordIds, ',') part
			WHERE pa.PARecordId = part.val
			)
END