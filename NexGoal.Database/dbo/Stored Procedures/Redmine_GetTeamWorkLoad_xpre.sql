
CREATE PROCEDURE [dbo].[Redmine_GetTeamWorkLoad_xpre] 
@EmployeeId INT
,@FromDate DATETIME
,@ToDate DATE
AS
/*
Created: 20181001@Oliver.Dang
Task: Get member workload under leader
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	declare @members as table (EmployeeId INT)

	insert into @members
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@EmployeeId)

	SELECT g.GroupId
		 ,g.Name as GroupName
		 ,e.EmployeeId as EmpId
		 ,e.NickName
		 ,SUM(ps.[Hours]) as TotalHour
		 ,p.Name as ProjectName
		 ,p.ProjectId
	FROM [dbo].Employee as e WITH (NOLOCK)
	INNER JOIN @members AS m ON e.EmployeeId = m.EmployeeId
	INNER JOIN [dbo].[Group] as g WITH (NOLOCK) ON e.GroupId = g.GroupId
	LEFT JOIN [dbo].ProjectSpentTime as ps WITH (NOLOCK) ON ps.EmployeeId = e.EmployeeId
	LEFT JOIN [dbo].Project as p WITH (NOLOCK) ON ps.ProjectId = p.ProjectId
	WHERE 
		(ps.SpentOn BETWEEN @FromDate AND @ToDate)
		AND e.EmployeeId <> @EmployeeId
	GROUP BY p.ProjectId
			,p.Name
			,g.GroupId
			,g.Name
			,e.EmployeeId
			,e.NickName
END
