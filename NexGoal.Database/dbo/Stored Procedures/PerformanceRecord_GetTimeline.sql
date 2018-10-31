
CREATE PROCEDURE [dbo].[PerformanceRecord_GetTimeline] @EmployeeId INT
	,@PageIndex INT = 0
	,@PageSize INT = 8
	,@TotalItems INT OUT
AS
/*
Created: 20181024@Tim.Trinh
Task: Get timeline.
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

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,CreatorId INT
		,GradeId VARCHAR(MAX)
		,StatusId TINYINT
		,RoleId TINYINT
		,CreatedDate DATETIME
		)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,CreatorId
		,GradeId
		,StatusId
		,RoleId
		,CreatedDate
		)
	SELECT record.PerformanceRecordId
		,record.CreatorId
		,record.GradeId
		,(CASE 
			WHEN record.CreatorId != @EmployeeId THEN @Closed
			ELSE record.StatusId END
			) AS StatusId
		,record.RoleId
		,record.CreatedDate
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		AND creator.[Enabled] = 1
		AND creator.IsDeleted = 0
	WHERE record.TypeId = 1
		AND record.StatusId IN (@Submitted, @Closed)

	SELECT PerformanceRecordId
		,CreatorId
		,GradeId
		,StatusId
		,RoleId
		,CreatedDate
	FROM #tblPerformanceRecord
	ORDER BY CreatedDate DESC OFFSET(@PageIndex * @PageSize) ROWS

	FETCH NEXT @PageSize ROWS ONLY

	SELECT @TotalItems = COUNT(1)
	FROM #tblPerformanceRecord

	DROP TABLE #tblPerformanceRecord
END
