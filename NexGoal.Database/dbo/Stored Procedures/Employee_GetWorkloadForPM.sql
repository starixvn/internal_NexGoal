
CREATE PROCEDURE [dbo].[Employee_GetWorkloadForPM] @EmployeeId INT
AS
/*
Created: 20180926@Elton.ton
Task: Get employee workload for PM
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @nickName VARCHAR(200)
	DECLARE @currentDate DATE = GETDATE()
	DECLARE @fromDate DATE = DATEADD(MONTH, -6, @currentDate)

	SELECT @nickName = e.NickName
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE e.EmployeeId = @EmployeeId

	SELECT pst.ProjectId
		,p.[Name]
		,SUM(pst.[Hours]) AS TotalHours
	FROM dbo.ProjectSpentTime AS pst WITH (NOLOCK)
		INNER JOIN dbo.Project AS p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.ProjectMember AS pm WITH (NOLOCK)
			WHERE pm.RoleId = 3 AND pm.ProjectId = p.ProjectId
				AND pm.EmployeeId = @EmployeeId
			)
		AND pst.SpentOn BETWEEN @fromDate AND @currentDate
	GROUP BY pst.ProjectId
		,p.[Name]
END