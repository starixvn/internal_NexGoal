


CREATE PROCEDURE [dbo].[Violation_Get] @FromDate DATETIME
	,@ToDate DATETIME
	,@EmployeeId INT = NULL
	,@GroupIds NVARCHAR(max) = NULL
	,@IsActive INT = NULL
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

	DECLARE @Groups INTID

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO @Groups (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@GroupIds, ',');
	END;

	SELECT v.ViolationId
		,v.[Description]
		,v.IsActive
		,v.EffectedDate
		,g.NAME AS GroupName
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,c.EmployeeId AS CreatedBy
		,c.NickName
		,c.FullName
	FROM dbo.Violation AS v WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = v.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Employee AS c WITH (NOLOCK) ON c.EmployeeId = v.CreatedBy
	WHERE v.EffectedDate BETWEEN CONVERT(date, @FromDate) 
			AND CONVERT(date, @ToDate) 
		AND (
			@EmployeeId IS NULL
			OR v.EmployeeId = @EmployeeId
			)
		AND (
			@IsActive IS NULL
			OR v.IsActive = @IsActive
			)
		AND (
			@GroupIds IS NULL
			OR g.GroupId IN (
				SELECT REPLACE(Id, '-', '')
				FROM @Groups
				WHERE Id LIKE '-%'
				)
			)
END