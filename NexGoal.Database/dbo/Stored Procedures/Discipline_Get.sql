


CREATE PROCEDURE [dbo].[Discipline_Get] @FromDate DATETIME
	,@ToDate DATETIME
	,@EmployeeId INT = NULL
	,@GroupIds NVARCHAR(max) = NULL
	,@IsActive INT = NULL
AS
/*
Created: 20180716@Elton Ton
Task: Discipline management.
DB: NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT d.DisciplineId
		,d.[Description]
		,d.IsActive
		,d.EffectiveDate
		,g.Name AS [Group]
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,c.EmployeeId AS CreatedBy
		,c.NickName AS CreatorNickName
		,c.FullName AS CreatorFullName
	FROM dbo.Discipline AS d WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = d.EmployeeId
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
	INNER JOIN dbo.Employee AS c WITH (NOLOCK) ON c.EmployeeId = d.CreatedBy
	WHERE d.EffectiveDate BETWEEN @FromDate	AND @ToDate
	AND (@EmployeeId IS NULL OR d.EmployeeId = @EmployeeId)
	AND (@IsActive IS NULL OR d.IsActive = @IsActive)
	AND (@GroupIds IS NULL OR EXISTS(SELECT 1 FROM dbo.func_SplitString(@GroupIds, ',') AS groups WHERE g.GroupId = groups.val))
	ORDER BY d.EffectiveDate DESC
END
