
CREATE PROCEDURE [dbo].[Employee_GetWorkloadForLeader] @EmployeeId INT
AS
/*
Created: 20180926@Elton.ton
Task: Get employee workload for leader
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @currentDate DATE = GETDATE()
	DECLARE @fromDate DATE = DATEADD(MONTH, -6, @currentDate)

	SELECT pst.ProjectId
		,p.[Name]
		,SUM(pst.[Hours]) AS TotalHours
	FROM dbo.ProjectSpentTime AS pst WITH (NOLOCK)
		INNER JOIN dbo.Project AS p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@EmployeeId) m
			WHERE pst.EmployeeId = m.EmployeeId AND m.ProcessId = 4
			)
		AND pst.SpentOn BETWEEN @fromDate AND @currentDate
	GROUP BY pst.ProjectId
		,p.[Name] 
END

