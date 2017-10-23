
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
		,@ProcessBySelf TINYINT = 2
		,@ProcessByEvaluator TINYINT = 3
		,@ProcessByReviewer TINYINT = 4
		,@ProbationTargetType TINYINT = 1
		,@SemiTargetType TINYINT = 3

	SELECT data.*
	FROM (
		SELECT pc.EPAId
			,NULL AS NickName
			,NULL AS FullName
			,period.PeriodId
			,period.[Name] AS PeriodName
			,period.TargetTypeId
			,pc.ProcessId
			,pc.EndDate
			,0 AS Members
			,0 AS Unfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.EvaluatorId = @EmployeeId
			AND pc.ProcessId = @ProcessBySelf
			AND @Today BETWEEN pc.StartDate
				AND pc.EndDate
		WHERE ISNULL(epa.[Status], 0) = @Inprogress
		
		UNION
		SELECT epa.EPAId
			,emp.NickName
			,emp.FullName
			,period.PeriodId
			,period.[Name] AS PeriodName
			,period.TargetTypeId
			,pc.ProcessId
			,pc.EndDate AS EndDate
			,1 AS Members
			,(CASE 
				WHEN ISNULL(pc.TotalScore, '') <> '' THEN 0
				ELSE 1 END
				) AS Unfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			AND period.TargetTypeId = @ProbationTargetType
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.EvaluatorId = @EmployeeId
			AND pc.ProcessId IN (@ProcessByEvaluator, @ProcessByReviewer)
			AND @Today BETWEEN pc.StartDate
				AND pc.EndDate
		WHERE ISNULL(epa.[Status], 0) = @Inprogress
		
		UNION
		SELECT 0 AS EPAId
			,NULL AS NickName
			,NULL AS FullName
			,period.PeriodId
			,period.[Name] AS PeriodName
			,period.TargetTypeId
			,pc.ProcessId
			,MIN(pc.EndDate) AS EndDate
			,COUNT(1) AS Members
			,SUM(CASE 
					WHEN ISNULL(pc.TotalScore, '') <> '' THEN 0
					ELSE 1
					END) AS Unfinished
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
			AND period.IsActive = 1
			AND period.TargetTypeId = @SemiTargetType
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
			AND pc.EvaluatorId = @EmployeeId
			AND pc.ProcessId IN (@ProcessByEvaluator, @ProcessByReviewer)
			AND @Today BETWEEN pc.StartDate
				AND pc.EndDate
		WHERE ISNULL(epa.[Status], 0) = @Inprogress
		GROUP BY period.PeriodId
			,period.[Name]
			,period.TargetTypeId
			,pc.ProcessId
		) AS data
	ORDER BY data.TargetTypeId
		,data.ProcessId
		,data.EndDate

	-- nomination
	SELECT data.EOQYId
		,data.[Year]
		,data.[Quarter]
		,data.IsQuarter
		,data.RoleId
		,data.EndDate
	FROM (
		SELECT DISTINCT eoqy.EOQYId
			,eoqy.[Year]
			,eoqy.[Quarter]
			,eoqy.IsQuarter
			,f.val AS RoleId
			,eoqy.EndDate
		FROM dbo.EOQY AS eoqy WITH (NOLOCK)
			INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON eoqy.EOQYId = setting.EOQYId
			AND setting.NominatorId = @EmployeeId
			CROSS APPLY dbo.fnc_strSplit(setting.RoleId, ',') AS f
		WHERE f.val = 1
			AND @Today BETWEEN eoqy.StartDate
				AND eoqy.EndDate
		
		UNION
		SELECT DISTINCT eoqy.EOQYId
			,eoqy.[Year]
			,eoqy.[Quarter]
			,eoqy.IsQuarter
			,f.val AS RoleId
			,eoqy.EndDateForCommittee AS EndDate
		FROM dbo.EOQY AS eoqy WITH (NOLOCK)
			INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON eoqy.EOQYId = setting.EOQYId
			AND setting.NominatorId = @EmployeeId
			CROSS APPLY dbo.fnc_strSplit(setting.RoleId, ',') AS f
		WHERE f.val = 2
			AND @Today > eoqy.EndDate
			AND @Today <= eoqy.EndDateForCommittee
		) AS data
	ORDER BY data.IsQuarter
		,data.[Year] DESC
		,data.[Quarter] DESC

	-- focus
	SELECT DISTINCT p.PeriodId
		,p.[Name] AS PeriodName
		,fp.EndDate
	FROM dbo.PAFocus AS f WITH (NOLOCK)
		INNER JOIN dbo.PAFocusProcess AS fp WITH (NOLOCK) ON f.PAFocusId = fp.PAFocusId
		AND @Today BETWEEN fp.StartDate
			AND fp.EndDate
		AND fp.ProcessId = @ProcessByEvaluator
		AND fp.EvaluatorId = @EmployeeId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON f.PeriodId = p.PeriodId
		AND p.IsActive = 1
END