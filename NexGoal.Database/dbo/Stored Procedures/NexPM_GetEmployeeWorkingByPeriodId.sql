


CREATE PROCEDURE [dbo].[NexPM_GetEmployeeWorkingByPeriodId] @PeriodId INT
	,@EmployeeId INT = NULL
AS
/*
Created: 20160804@Razor.nguyen
Task: PA Form
DB: NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SELECT pm.ProjectName,
                pm.ProjectId,
                pm.StartDate,
                pm.EndDate,
                pm.Roles,
                pm.Score AS EncryptedScore,
                pm.Grade AS EncryptedGrade,
                pm.Comment,
                pm.EmployeeId
	FROM dbo.Period AS p WITH (NOLOCK)
		INNER JOIN dbo.NexPM AS pm WITH (NOLOCK) ON pm.EndDate >= p.AttendanceStartDate
		AND (
			pm.EndDate <= p.AttendanceEndDate
			OR pm.EndDate IS NULL
			)
	WHERE p.PeriodId = @PeriodId
		AND (
			@EmployeeId IS NULL
			OR pm.EmployeeId = @EmployeeId
			)
END;