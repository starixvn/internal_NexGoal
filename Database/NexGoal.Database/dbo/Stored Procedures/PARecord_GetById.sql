
CREATE PROCEDURE [dbo].[PARecord_GetById] @ActorId INT
	,@PARecordId INT
AS
/*
Created: 20180920@Tim.Trinh
Task: Get PA by id.
DB: bodb_NexGoal
Original:

Revisions:
    20190117@Tim.Trinh: refactor
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
		,@ReviewerId INT

	SELECT pa.PARecordId
		,pa.ModifiedDate
		,pa.OriginalGradeId
        ,pa.Toeic
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
	WHERE pa.PARecordId = @PARecordId

	SELECT process.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,process.ProcessId AS RoleId
		,record.TypeId
		,record.CreatorId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE process.PARecordId = @PARecordId
		AND process.ProcessId NOT IN (@HRProcessId, @PMProcessId)
END
