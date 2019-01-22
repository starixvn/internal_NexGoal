CREATE PROCEDURE [dbo].[Discipline_GetByEmployeeAndPeriod] 
	@EmployeeId INT = NULL,
	@PeriodId INT = NULL
AS
/*
Created: 20190121@Oliver.Dang
Task: get discipline in by period.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME

	SELECT
		@FromDate = DisciplineStartDate
		,@ToDate = DisciplineEndDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT d.DisciplineId
		,d.[Description]
		,d.EffectiveDate
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,c.EmployeeId AS CreatedBy
		,c.NickName AS CreatorNickName
		,c.FullName AS CreatorFullName
	FROM dbo.Discipline AS d WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = d.EmployeeId
		INNER JOIN dbo.Employee AS c WITH (NOLOCK) ON c.EmployeeId = d.CreatedBy
	WHERE d.EffectiveDate BETWEEN @FromDate
			AND @ToDate
		AND d.EmployeeId = @EmployeeId
		AND d.IsActive = 1
	ORDER BY d.EffectiveDate DESC
		,d.CreatedDate
END
