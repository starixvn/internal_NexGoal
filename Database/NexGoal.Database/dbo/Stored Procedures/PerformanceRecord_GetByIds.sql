


CREATE PROCEDURE [dbo].[PerformanceRecord_GetByIds] @PerformanceRecordIds VARCHAR(MAX)
	,@CreatorId INT
AS
/*
Created: 20181127@Tim.Trinh
Task: Get PCI by ids.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblPerformanceRecordIds (PerformanceRecordId INT)

	INSERT INTO #tblPerformanceRecordIds (PerformanceRecordId)
	SELECT val
	FROM dbo.func_SplitString(@PerformanceRecordIds, ',')
	WHERE ISNUMERIC(val) = 1

	SELECT
		record.PerformanceRecordId
		,GradeId
		,Comment
		,0 AS IsDraft
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,e.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,cre.EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
	FROM #tblPerformanceRecordIds AS per
		INNER JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON per.PerformanceRecordId = record.PerformanceRecordId
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON record.EmployeeId = e.EmployeeId
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	ORDER BY CreatedDate DESC

	SELECT record.PerformanceRecordId
		,prj.ProjectId
		,prj.[Name] 
	FROM #tblPerformanceRecordIds AS per
		INNER JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON per.PerformanceRecordId = record.PerformanceRecordId
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.Project AS prj WITH (NOLOCK) ON a.val = prj.ProjectId


	--SELECT record.PerformanceRecordId
	--	,record.GradeId
	--	,record.Comment
	--	,record.ProjectIds
	--	,record.CreatedDate
	--	,record.ModifiedDate
	--	,record.EmployeeId
	--	,record.CreatorId
	--	,record.TypeId
	--	,record.RoleId
	--FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
	--	INNER JOIN #tblPerformanceRecordIds AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.PerformanceRecordId
	--	INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	--WHERE record.CreatorId = @CreatorId
	--	AND record.TypeId = 1
	--ORDER BY record.CreatedDate

	DROP TABLE #tblPerformanceRecordIds
END