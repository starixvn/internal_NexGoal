
CREATE PROCEDURE [dbo].[PerformanceRecord_GetByEmployee] @EmployeeId INT
AS
/*
Created: 20180912@Tim.Trinh
Task: Get PCI by employee.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	-- Performance Record Status
	DECLARE @Draft TINYINT = 2
		,@Submitted TINYINT = 3
		,@Closed TINYINT = 4

	SELECT data.PerformanceRecordId
		,data.GradeId
		,data.Comment
		,data.StatusId
		,data.ProjectIds
		,data.CreatedDate
		,data.ModifiedDate
		,data.RoleId
		,data.EmployeeIds
		,data.CreatorId
	FROM (
		SELECT record.PerformanceRecordId
			,record.GradeId
			,record.Comment
			,record.StatusId
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,record.EmployeeIds
			,record.CreatorId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = 1
			AND record.CreatorId = @EmployeeId
			AND CAST(record.CreatorId AS VARCHAR(20)) = record.EmployeeIds
			AND record.StatusId = @Draft
		
		UNION
		SELECT record.PerformanceRecordId
			,record.GradeId
			,record.Comment
			,(CASE 
				WHEN record.CreatorId != @EmployeeId THEN @Closed
				ELSE record.StatusId END
				) AS StatusId
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,record.EmployeeIds
			,record.CreatorId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
			AND emp.EmployeeId = @EmployeeId
			INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
			AND creator.[Enabled] = 1
			AND creator.IsDeleted = 0
		WHERE record.TypeId = 1
			AND record.StatusId IN (@Submitted, @Closed)
		) AS data
	ORDER BY CASE 
			WHEN data.StatusId = @Draft THEN 1
			ELSE 2
			END
		,data.CreatedDate DESC
END