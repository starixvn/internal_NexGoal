
CREATE PROCEDURE [dbo].[PARecord_GetDisciplineById] @PARecordId INT
AS
/*
Created: 20190125@Tim.Trinh
Task: get discipline.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeId INT
		,@FromDate DATETIME
		,@ToDate DATETIME

	SELECT @EmployeeId = pa.EmployeeId
		,@FromDate = DisciplineStartDate
		,@ToDate = DisciplineEndDate
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON period.PeriodId = pa.PeriodId
	WHERE pa.PARecordId = @PARecordId

	SELECT DisciplineId
		,[Description]
		,EffectiveDate
	FROM dbo.Discipline WITH (NOLOCK)
	WHERE EmployeeId = @EmployeeId
		AND EffectiveDate BETWEEN @FromDate
			AND @ToDate
		AND IsActive = 1
	ORDER BY EffectiveDate DESC
		,CreatedDate
END