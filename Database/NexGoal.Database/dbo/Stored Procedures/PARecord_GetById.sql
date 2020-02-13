CREATE PROCEDURE [dbo].[PARecord_GetById] @ActorId INT
	,@PARecordId INT
AS
/*
Created: 20180920@Tim.Trinh
Task: Get PA by id.
DB: bodb_NexGoal
Original:

Revisions:
    20190129@Oliver.Dang: fix bugs
    20191118@elton.ton: get process name
	20200212@Tim.Trinh: get related performance record
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@HRProcessId TINYINT = 1
		,@PMProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 5
		-- PA status
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		-- role
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
	DECLARE @tblRelatedPerformanceRecord TABLE (
		PARecordId INT
		,ProcessId TINYINT
		,PerformanceRecordId INT
		)

	SELECT pa.PARecordId
		,pa.ModifiedDate
		,pa.IsRevised
		,pa.OriginalGradeId
		,pa.Toeic
		,pa.ToeicPolicy
		,DATEADD(year, 3, t.LRExamDate) ToeicExpiredDate
		,(CASE 
			WHEN @Today BETWEEN reviewer.StartDate
					AND reviewer.EndDate THEN @PAInProgress
			ELSE @PAClosed END
			) AS StatusId
		,pa.GradeId
		,pa.Comment
		,pa.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,pa.PeriodId
		,period.[Name]
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
		INNER JOIN dbo.TOEIC AS t WITH (NOLOCK) ON pa.EmployeeId = t.EmpId
	WHERE pa.PARecordId = @PARecordId

	SELECT process.PARecordId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
		,process.ProcessId
		,rr.Value AS Process
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON process.ProcessId = rr.ResourceId
		AND rr.RefName = 'PerformanceProcess'
	WHERE process.PARecordId = @PARecordId
		AND process.ProcessId > @HRProcessId

	INSERT INTO @tblRelatedPerformanceRecord (
		PARecordId
		,ProcessId
		,PerformanceRecordId
		)
	SELECT process.PARecordId
		,process.ProcessId
		,child.val
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		CROSS APPLY [dbo].[func_SplitString](process.RelatedPerformanceRecordIds, ',') AS child
	WHERE process.PARecordId = @PARecordId
		AND process.ProcessId > @HRProcessId

	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,creator.EmployeeId
		,creator.Code
		,creator.NickName
		,creator.FullName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN @tblRelatedPerformanceRecord AS tbl ON record.PerformanceRecordId = tbl.PerformanceRecordId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId

	SELECT record.PerformanceRecordId
		,pt.ProjectId
		,pt.[Name]
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN @tblRelatedPerformanceRecord AS tbl ON record.PerformanceRecordId = tbl.PerformanceRecordId
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') AS pr
		INNER JOIN dbo.ProjectTag AS pt WITH (NOLOCK) ON pr.val = pt.ProjectId
END
