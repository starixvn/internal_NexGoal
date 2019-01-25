


CREATE PROCEDURE [dbo].[DraftPerformanceRecord_GetById] @PerformanceRecordId INT
AS
/*
Created: 20190104@Quinn.Vo
Task: Get Draft PCI by id.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT
		record.DraftPerformanceRecordId AS PerformanceRecordId
		,GradeId
		,Comment
		,1 AS IsDraft
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,cre.EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
	FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	WHERE record.DraftPerformanceRecordId = @PerformanceRecordId

	SELECT record.DraftPerformanceRecordId AS PerformanceRecordId
		,prj.ProjectId
		,prj.[Name] 
	FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.Project AS prj WITH (NOLOCK) ON a.val = prj.ProjectId
	WHERE record.DraftPerformanceRecordId = @PerformanceRecordId

	SELECT e.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
	FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
		OUTER APPLY [dbo].[func_SplitString](record.EmployeeIds, ',') a
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON a.val = e.EmployeeId
	WHERE record.DraftPerformanceRecordId = @PerformanceRecordId
END