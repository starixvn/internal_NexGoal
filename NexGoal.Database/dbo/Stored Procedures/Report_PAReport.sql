
CREATE PROCEDURE [dbo].[Report_PAReport] @UserId INT
	,@EmployeeId INT = NULL
	,@PeriodIds NVARCHAR(max) = NULL
	,@GroupIds NVARCHAR(max) = NULL
AS
/*
Created: 20151001@Tim.Trinh
Task: EPA Detail Report.
DB: SMSEPA
Original: @SortDirection = 'asc'/'desc'

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Phases IntId
		,@ToDay DATE
		,@Ids NVARCHAR(MAX)
		,@SortCode NVARCHAR(10)
		,@SortSection NVARCHAR(10)

	CREATE TABLE #Groups (GroupId INT)

	CREATE TABLE #EPAs (EPAId INT)

	CREATE TABLE #Periods (
		PeriodId INT
		,[Name] NVARCHAR(200)
		,StartDate DATETIME
		,EndDate DATETIME
		,TargetTypeId INT
		,TargetName NVARCHAR(200)
		)

	CREATE TABLE #TblPeriods (
		PeriodId INT
		,[Name] NVARCHAR(200)
		)

	SET @ToDay = CAST(GETDATE() AS DATE)
	SET @SortCode = 'code'
	SET @SortSection = 'section'

	INSERT INTO #Groups
	SELECT DISTINCT val
	FROM fnc_strSplit(@GroupIds, ',')

	INSERT INTO #Periods
	EXECUTE dbo.EpaPeriod_GetAllPeriodIncludeImport

	INSERT INTO #TblPeriods
	SELECT PeriodId
		,[Name]
	FROM #Periods
	WHERE @PeriodIds IS NULL
		OR PeriodId IN (
			SELECT DISTINCT val
			FROM fnc_strSplit(@PeriodIds, ',')
			)

	INSERT INTO #EPAs
	SELECT epa.EPAId
	FROM (
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN #TblPeriods AS p ON p.PeriodId = epa.PeriodId
		WHERE EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS ro WITH (NOLOCK)
					INNER JOIN dbo.LookupData AS lo WITH (NOLOCK) ON ro.RoleId = lo.Value
					AND lo.LookupId = 2 -- role
				WHERE ro.EmployeeId = @UserId
					AND ro.RoleId IN (1, 2, 3) -- SA, HR, MGT can view all member
				)
		
		UNION
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			INNER JOIN #TblPeriods AS p ON p.PeriodId = epa.PeriodId
		WHERE pc.ProcessId = 3 --evaluator
			AND pc.EvaluatorId = @Userid
			AND (
				epa.[Status] = 1
				OR (
					epa.[Status] = 0
					AND pc.StartDate <= GETDATE()
					)
				)
		
		UNION
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON pc.EPAId = epa.EPAId
			INNER JOIN #TblPeriods AS p ON p.PeriodId = epa.PeriodId
		WHERE pc.ProcessId = 4 --evaluator
			AND pc.EvaluatorId = @Userid
			AND (
				epa.[Status] = 1
				OR (
					epa.[Status] = 0
					AND pc.StartDate <= GETDATE()
					)
				)
		) AS TEMP
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON epa.EPAId = TEMP.EPAId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = epa.EmployeeId
	WHERE (
			@EmployeeId IS NULL
			OR e.EmployeeId = @EmployeeId
			)
		AND (
			@GroupIds IS NULL
			OR e.GroupId IN (
				SELECT REPLACE(GroupId, '-', '')
				FROM #Groups
				WHERE GroupId LIKE '-%'
				)
			)
	ORDER BY epa.PeriodId DESC

	-- General Info
	SELECT epa.EPAId
		,epa.FinalScore
		,epa.Grade
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.JoinedDate
		,lkd.[Name] AS Category
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.Id AS JobTitleId
		,job.[Name] AS JobTitleName
		,epa.[Status] AS StatusId
		,period.PeriodId
		,period.[Name]
		,(CASE 
			WHEN ttype.TargetTypeId IN (2, 3) THEN (CASE 
					WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN (CASE 
							WHEN CAST(pc.EndDate AS DATE) < @ToDay THEN 1
							ELSE 0 END
							)
					ELSE (CASE 
							WHEN CAST(period.EndDate AS DATE) <= @ToDay THEN 1
							ELSE 0 END
							) END
					)
			ELSE (CASE 
					WHEN CAST(pc.EndDate AS DATE) < @ToDay THEN 1
					ELSE 0 END
					) END
			) AS IsEndPeriod
		,ttype.TargetTypeId
		,ttype.[Name] AS TargetTypeName
		,srt.ScoreRatingTypeId
		,srt.SelfEvaluated
		,srt.Evaluator
		,srt.Reviewer
		,srt.Management
	FROM #EPAs AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN (
		SELECT pc1.EPAId
			,MIN(pc1.StartDate) AS StartDate
			,MAX(pc1.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
			INNER JOIN #EPAs AS tbl1 ON pc1.EPAId = tbl1.EPAId
		GROUP BY pc1.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
		LEFT JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON emp.IsLocal = CAST(lkd.Value AS BIT)
		AND lkd.LookupId = 11 -- employee category
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.TargetType AS ttype WITH (NOLOCK) ON period.TargetTypeId = ttype.TargetTypeId
	ORDER BY period.PeriodId
		,epa.EPAId DESC

	-- Process
	SELECT DISTINCT epa.EPAId
		,pc.ProcessId
		,pc.TotalScore
		,pc.IsEndProcess
		,(CASE 
			WHEN ISNULL(epa.EPAId, '') <> '' THEN CASE 
					WHEN pc.IsEndProcess = 0 THEN CASE 
							WHEN CAST(pc.StartDate AS DATE) > CAST(GETDATE() AS DATE) THEN 1 -- Open
							ELSE 2 -- In Progress
							END
					ELSE 3 -- Closed
					END
			ELSE 0 END
			) AS StatusEndProcess
		,emp.EmployeeId AS EvaluatorId
		,emp.Code
		,emp.NickName
		,emp.FullName
	FROM #EPAs AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.EPAId = epa.EPAId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId > 1 -- ignore HR phase
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pc.EvaluatorId = emp.EmployeeId

	DROP TABLE #Groups

	DROP TABLE #EPAs

	DROP TABLE #Periods

	DROP TABLE #TblPeriods
END