CREATE PROCEDURE [dbo].[Dashboard_GetToDoList] @EmployeeId INT
AS
/*
Created: 20170929@Tim.Trinh
Task: Get to do list.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Today DATE = GETDATE()
		,@Inprogress TINYINT = 0
		,@ProcessByEvaluator TINYINT = 3
		,@ProcessByReviewer TINYINT = 4

	CREATE TABLE #tblPerformanceAppraisal (
		PeriodId INT
		,PeriodName NVARCHAR(200)
		,TargetTypeId TINYINT
		,UnfinishedByEvaluator INT
		,FinishedByEvaluator INT
		,UnfinishedByReviewer INT
		,FinishedByReviewer INT
		)

	INSERT INTO #tblPerformanceAppraisal (
		PeriodId
		,PeriodName
		,TargetTypeId
		)
	SELECT DISTINCT period.PeriodId
		,period.[Name]
		,period.TargetTypeId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.EvaluatorId = @EmployeeId
		AND pc.ProcessId IN (3, 4) -- evaluator, reviewer
		AND @Today BETWEEN pc.StartDate
			AND pc.EndDate
	WHERE ISNULL(epa.[Status], 0) = @Inprogress
	ORDER BY period.TargetTypeId

	-- evaluator
	UPDATE pa
	SET pa.UnfinishedByEvaluator = tbl.Unfinished
		,pa.FinishedByEvaluator = tbl.Finished
	FROM #tblPerformanceAppraisal AS pa
		INNER JOIN (
		SELECT epa.PeriodId
			,SUM(CASE 
					WHEN pc.TotalScore IS NOT NULL THEN 1
					ELSE 0
					END) AS Finished
			,SUM(CASE 
					WHEN pc.TotalScore IS NULL THEN 1
					ELSE 0
					END) AS Unfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.EvaluatorId = @EmployeeId
			AND pc.ProcessId = @ProcessByEvaluator
			AND @Today BETWEEN pc.StartDate
				AND pc.EndDate
		WHERE ISNULL(epa.[Status], 0) = @Inprogress
		GROUP BY epa.PeriodId
		) AS tbl ON pa.PeriodId = tbl.PeriodId

	-- reviewer
	UPDATE pa
	SET pa.UnfinishedByReviewer = tbl.Unfinished
		,pa.FinishedByReviewer = tbl.Finished
	FROM #tblPerformanceAppraisal AS pa
		INNER JOIN (
		SELECT epa.PeriodId
			,SUM(CASE 
					WHEN pc.TotalScore IS NOT NULL THEN 1
					ELSE 0
					END) AS Finished
			,SUM(CASE 
					WHEN pc.TotalScore IS NULL THEN 1
					ELSE 0
					END) AS Unfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.EvaluatorId = @EmployeeId
			AND pc.ProcessId = @ProcessByReviewer
			AND @Today BETWEEN pc.StartDate
				AND pc.EndDate
		WHERE ISNULL(epa.[Status], 0) = @Inprogress
		GROUP BY epa.PeriodId
		) AS tbl ON pa.PeriodId = tbl.PeriodId

	------
	SELECT PeriodId
		,PeriodName
		,UnfinishedByEvaluator
		,FinishedByEvaluator
		,UnfinishedByReviewer
		,FinishedByReviewer
	FROM #tblPerformanceAppraisal WITH (NOLOCK)

	SELECT eoqy.EOQYId
		,eoqy.[Year]
		,eoqy.[Quarter]
		,setting.RoleId
		,eoqy.StartDate
		,eoqy.EndDate
		,eoqy.EndDateForCommittee
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON eoqy.EOQYId = setting.EOQYId
		AND setting.NominatorId = @EmployeeId
		CROSS APPLY dbo.fnc_strSplit(setting.RoleId, ',') AS f
	WHERE (
			(
				f.val = 1
				AND @Today BETWEEN eoqy.StartDate
					AND eoqy.EndDate
				)
			OR (
				f.val = 2
				AND @Today > eoqy.EndDate
				AND @Today <= eoqy.EndDateForCommittee
				)
			)
	ORDER BY eoqy.IsQuarter
		,eoqy.[Year] DESC
		,eoqy.[Quarter] DESC

	SELECT DISTINCT p.PeriodId
		,p.[Name] AS PeriodName
		,fp.StartDate
		,fp.EndDate
	FROM dbo.PAFocus AS f WITH (NOLOCK)
		INNER JOIN dbo.PAFocusProcess AS fp WITH (NOLOCK) ON f.PAFocusId = fp.PAFocusId
		AND @Today BETWEEN fp.StartDate
			AND fp.EndDate
		AND fp.ProcessId = 3
		AND fp.EvaluatorId = @EmployeeId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON f.PeriodId = p.PeriodId
		AND p.IsActive = 1

	DROP TABLE #tblPerformanceAppraisal
END