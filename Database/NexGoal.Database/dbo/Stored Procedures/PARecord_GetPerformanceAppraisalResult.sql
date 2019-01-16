
CREATE PROCEDURE [dbo].[PARecord_GetPerformanceAppraisalResult] @EmployeeId INT
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
	DECLARE @PAClosed TINYINT = 4

	SELECT pa.PARecordId
		,pa.GradeId
		,period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate		
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
	WHERE pa.EmployeeId = @EmployeeId
		AND pa.StatusId = @PAClosed
END