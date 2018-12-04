
CREATE PROCEDURE [dbo].[PerformanceRecord_GetById] @PerformanceRecordId INT
	,@CreatorId INT
AS
/*
Created: 20180912@Tim.Trinh
Task: Get PCI by id.
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
		,record.RoleId
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.PerformanceRecordId = @PerformanceRecordId
		AND record.CreatorId = @CreatorId
		AND record.TypeId = 1
END
