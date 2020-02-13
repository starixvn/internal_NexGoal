CREATE PROCEDURE [dbo].[PARecord_GetByReviewer] @ActorId INT
	,@PeriodId INT
	,@GroupIds VARCHAR(MAX) = ''
	,@IsRevised BIT = NULL
AS
/*
Created: 20181127@Oliver.Dang
Task: Get PA by Reviewer.
DB: bodb_NexGoal
Original:

Revisions:
    20190117@Tim.Trinh: refactor
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		-- PA status
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	CREATE TABLE #tblGroupId (GroupId INT)

	CREATE TABLE #tblPARecordId (PARecordId INT)

	INSERT INTO #tblGroupId (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblPARecordId (PARecordId)
	SELECT pa.PARecordId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
	INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
	INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.EvaluatorId = @ActorId
	INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
	INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE pa.StatusId IN (
			@PAInProgress
			,@PAClosed
			)
		AND process.ProcessId = @ReviewerProcessId
		AND (
			@IsRevised IS NULL
			OR @IsRevised = ISNULL(pa.IsRevised,0)
			)
	GROUP BY pa.PARecordId

	SELECT pa.PARecordId
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,pa.IsRevised
		,pa.Toeic
		,(
			CASE 
				WHEN pa.StatusId = @PAInProgress
					AND (
						@Today BETWEEN p.StartDate
							AND p.EndDate
						)
					THEN @PAInProgress
				ELSE @PAClosed
				END
			) AS StatusId
		,pa.GradeId
		,pa.Comment
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,g.Name AS GroupName
	FROM dbo.PARecord AS pa WITH (NOLOCK)
	INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
	INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON emp.GroupId = g.GroupId
	INNER JOIN dbo.PAProcess AS p WITH (NOLOCK) ON pa.PARecordId = p.PARecordId
		AND p.ProcessId = @ReviewerProcessId
	INNER JOIN dbo.func_GetSortedEmployee(@ActorId) AS es ON es.EmployeeId = emp.EmployeeId
	ORDER BY es.RankIndex

	SELECT process.PARecordId
		,process.ProcessId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
	INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
	INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
	LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
	WHERE process.ProcessId >= @SelfProcessId

	DROP TABLE #tblGroupId

	DROP TABLE #tblPARecordId
END
