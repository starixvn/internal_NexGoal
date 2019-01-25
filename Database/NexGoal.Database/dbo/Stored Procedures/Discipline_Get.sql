
CREATE PROCEDURE [dbo].[Discipline_Get] @FromDate DATE
	,@ToDate DATE
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@IsActive INT = NULL
AS
/*
Created: 20180716@Elton.Ton
Task: Discipline management.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT d.DisciplineId
		,d.[Description]
		,d.IsActive
		,d.EffectiveDate
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,g.[Name] AS [GroupName]
		,c.EmployeeId
		,c.NickName
		,c.FullName
	FROM dbo.Discipline AS d WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = d.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Employee AS c WITH (NOLOCK) ON c.EmployeeId = d.CreatedBy
	WHERE d.EffectiveDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(@EmployeeId, d.EmployeeId) = d.EmployeeId
		AND ISNULL(@IsActive, d.IsActive) = d.IsActive
		AND ISNULL(@GroupId, g.GroupId) = g.GroupId
	ORDER BY d.EffectiveDate DESC
		,d.CreatedDate
END
