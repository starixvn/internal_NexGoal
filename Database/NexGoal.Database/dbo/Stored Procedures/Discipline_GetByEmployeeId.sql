
CREATE PROCEDURE [dbo].[Discipline_GetByEmployeeId] @EmployeeId INT
	,@PeriodId INT = NULL
AS
/*
Created: 20190125@Tim.Trinh
Task: get discipline by employee.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATETIME = GETDATE()
		,@FromDate DATETIME
		,@ToDate DATETIME

	IF @PeriodId IS NULL
	BEGIN
		SELECT TOP 1 @FromDate = DisciplineStartDate
			,@ToDate = DisciplineEndDate
		FROM dbo.Period WITH (NOLOCK)
		WHERE IsActive = 1
			AND DisciplineStartDate <= @Today
		ORDER BY DisciplineStartDate DESC
	END
	ELSE
	BEGIN
		SELECT @FromDate = DisciplineStartDate
			,@ToDate = DisciplineEndDate
		FROM dbo.Period WITH (NOLOCK)
		WHERE PeriodId = @PeriodId
	END

	SELECT DisciplineId
		,[Description]
		,EffectiveDate
	FROM dbo.Discipline WITH (NOLOCK)
	WHERE EffectiveDate BETWEEN @FromDate
			AND @ToDate
		AND EmployeeId = @EmployeeId
		AND IsActive = 1
	ORDER BY EffectiveDate DESC
		,CreatedDate
END