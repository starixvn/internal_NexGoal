
CREATE OR ALTER PROCEDURE [dbo].[PerformanceRecord_GetById] @PerformanceRecordId INT
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

	SELECT PerformanceRecordId
		,GradeId
		,Comment
		,0 AS IsDraft
		,CreatedDate
		,record.ModifiedDate
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
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON record.EmployeeId = e.EmployeeId
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	WHERE record.PerformanceRecordId = @PerformanceRecordId
		AND record.CreatorId = @CreatorId
		AND record.TypeId = 1

	SELECT record.PerformanceRecordId
		,prj.ProjectId
		,prj.[Name]
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.ProjectTag AS prj WITH (NOLOCK) ON a.val = prj.ProjectId
	WHERE record.PerformanceRecordId = @PerformanceRecordId
		AND record.CreatorId = @CreatorId
		AND record.TypeId = 1
END
