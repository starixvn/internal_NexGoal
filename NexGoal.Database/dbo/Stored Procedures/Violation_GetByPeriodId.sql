


CREATE PROCEDURE [dbo].[Violation_GetByPeriodId] @PeriodId INT
	,@EmployeeId INT
AS
/*
Created: 20160901@Razor.Nguyen
Task: Violation management.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT v.ViolationId
		,v.[Description]
		,v.IsActive
		,v.EffectedDate
		,v.GroupName
		,v.EmployeeId
		,v.NickName
		,v.FullName
		,v.CreatedBy
		,v.CreatedNickName
		,v.CreatedFullName
	FROM dbo.Period AS p WITH (NOLOCK)
		INNER JOIN (
		SELECT v.ViolationId
			,v.[Description]
			,v.IsActive
			,v.EffectedDate
			,g.NAME AS GroupName
			,e.EmployeeId
			,e.NickName
			,e.FullName
			,c.EmployeeId AS CreatedBy
			,c.NickName AS CreatedNickName
			,c.FullName AS CreatedFullName
		FROM dbo.Violation AS v WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = v.EmployeeId
			INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
			INNER JOIN dbo.Employee AS c WITH (NOLOCK) ON c.EmployeeId = v.CreatedBy
		WHERE @EmployeeId IS NULL
			OR v.EmployeeId = @EmployeeId
		) AS v ON v.EffectedDate BETWEEN p.AttendanceStartDate
			AND p.AttendanceEndDate
	WHERE p.PeriodId = @PeriodId
		AND v.IsActive = 1
END