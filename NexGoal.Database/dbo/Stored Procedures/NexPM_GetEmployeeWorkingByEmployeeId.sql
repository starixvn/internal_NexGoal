
CREATE PROCEDURE [dbo].[NexPM_GetEmployeeWorkingByEmployeeId] @EmployeeId INT
	,@StartDate DATETIME
	,@EndDate DATETIME
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
	SET NOCOUNT ON;
	
	SELECT pm.ProjectName
		,pm.ProjectId
		,pm.StartDate
		,pm.EndDate
		,pm.Roles
		,pm.Score AS EncryptedScore
		,pm.Comment
		,pm.EmployeeId
		,ISNULL(rm.[Hours], 0) AS WorkingHour
		,pm.projectManager AS ProjectManagerId
		,m.NickName AS ProjectManagerName
	FROM dbo.NexPM AS pm WITH (NOLOCK)
		INNER JOIN dbo.Employee AS m WITH (NOLOCK) ON m.EmployeeId = pm.projectManager
		LEFT JOIN (
		SELECT rm.ProjectId
			,SUM([Hours]) AS [Hours]
		FROM dbo.Redmine AS rm WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.NickName = rm.NickName
		WHERE e.EmployeeId = @EmployeeId
			AND rm.SpentOn BETWEEN @StartDate
				AND @EndDate
		GROUP BY rm.ProjectId
		) AS rm ON rm.ProjectId = pm.ProjectId
	WHERE pm.EndDate >= @StartDate
		AND (
			pm.EndDate <= @EndDate
			OR pm.EndDate IS NULL
			)
		AND pm.EmployeeId = @EmployeeId
END