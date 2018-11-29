
CREATE PROCEDURE [dbo].[Employee_GetPerformanceAppraisalResult] @EmployeeId INT
AS
/*
Created: 20180927@Tim.Trinh
Task: Get PA.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	SELECT pa.PARecordId
		,pa.PeriodId
		,(CASE 
			WHEN pa.StatusId = @PAInProgress THEN NULL
			ELSE pa.GradeId END
			) AS GradeId
		,(CASE 
			WHEN pa.StatusId = @PAInProgress THEN NULL
			ELSE pa.Comment END
			) AS Comment
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
	WHERE pa.EmployeeId = @EmployeeId
		AND pa.StatusId IN (@PAInProgress, @PAClosed)

	SELECT period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
	WHERE pa.EmployeeId = @EmployeeId
		AND pa.StatusId IN (@PAInProgress, @PAClosed)
END
