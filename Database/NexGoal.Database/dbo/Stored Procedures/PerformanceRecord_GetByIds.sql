
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

	-- Performance Record Status
	DECLARE @Submitted TINYINT = 3
		,@Closed TINYINT = 4

	CREATE TABLE #tblPerformanceRecordIds (PerformanceRecordId INT)

	INSERT INTO #tblPerformanceRecordIds (PerformanceRecordId)
	SELECT val
	FROM dbo.func_SplitString(@PerformanceRecordIds, ',')
	WHERE ISNUMERIC(val) = 1

	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,record.StatusId
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.EmployeeIds
		,record.CreatorId
		,record.TypeId
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN #tblPerformanceRecordIds AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.CreatorId = @CreatorId
		AND record.TypeId = 1
	ORDER BY record.CreatedDate

	DROP TABLE #tblPerformanceRecordIds
END