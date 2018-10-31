
CREATE PROCEDURE [dbo].[Employee_GetWorkload] 
	@EmployeeId INT,
	@PeriodId INT = NULL
AS
/*
Created: 20180926@Elton.ton
Task: Get employee workload
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @toDate DATE, @fromDate DATE

	IF @PeriodId IS NULL
	BEGIN
		SET @toDate = GETDATE()
		SET @fromDate = DATEADD(MONTH, -6, @toDate)
	END
	ELSE
	BEGIN
		SELECT @fromDate = p.DisciplineStartDate, @toDate = p.DisciplineEndDate
		FROM dbo.Period p WITH (NOLOCK)
		WHERE p.PeriodId = @PeriodId
	END

	SELECT pst.ProjectId
		,p.[Name]
		,SUM(pst.[Hours]) AS TotalHours
	FROM dbo.ProjectSpentTime pst WITH (NOLOCK)
		INNER JOIN dbo.Project p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
	WHERE pst.EmployeeId = @EmployeeId
		AND pst.SpentOn BETWEEN @fromDate AND @toDate
	GROUP BY pst.ProjectId
		,p.[Name]
END