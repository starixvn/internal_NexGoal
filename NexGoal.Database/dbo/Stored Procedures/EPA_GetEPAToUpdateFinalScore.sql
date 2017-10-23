
CREATE PROCEDURE [dbo].[EPA_GetEPAToUpdateFinalScore] @PeriodId INT
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20160329@Tim.Trinh
Task: Get EPA to update final score.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @TblEPAId IntId
		,@Yesterday DATE = DATEADD(DAY, - 1, GETDATE())
		,@TargetTypeId INT
		,@Today DATE = GETDATE()
		,@ProbationTarget TINYINT = 1
		,@SemiTarget TINYINT = 3

	SELECT @TargetTypeId = TargetTypeId
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND IsActive = 1

	IF @TargetTypeId IN (@SemiTarget)
	BEGIN
		INSERT INTO @TblEPAId (Id)
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON epa.PeriodId = pe.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE CAST(pe.EndDate AS DATE) <= @Today
			AND CAST(pc.EndDate AS DATE) < CAST(pe.EndDate AS DATE)
			AND pe.TargetTypeId IN (@SemiTarget)
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
			AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
		
		UNION
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE CAST(pc.EndDate AS DATE) <= @Yesterday
			AND CAST(pc.EndDate AS DATE) >= CAST(pe.EndDate AS DATE)
			AND pe.TargetTypeId IN (@SemiTarget)
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
			AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
	END
	ELSE IF @TargetTypeId IN (@ProbationTarget)
	BEGIN
		INSERT INTO @TblEPAId (Id)
		SELECT DISTINCT epa.EPAId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN (
			SELECT DISTINCT EPAId
				,MAX(EndDate) AS EndDate
			FROM dbo.EPAProcess WITH (NOLOCK)
			GROUP BY EPAId
			) AS pc ON epa.EPAId = pc.EPAId
			INNER JOIN dbo.Period AS pe WITH (NOLOCK) ON pe.PeriodId = epa.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
			AND emp.IsDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		WHERE CAST(pc.EndDate AS DATE) <= @Yesterday
			AND pe.TargetTypeId IN (@ProbationTarget)
			AND epa.[Status] IN (0, 1) -- open/closed
			AND pe.IsActive = 1
			AND pe.PeriodId = @PeriodId
			AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
			AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
	END

	-- General Info
	SELECT DISTINCT epa.EPAId
		,epa.FinalScore
		,(CASE 
			WHEN tt.TargetTypeId = @SemiTarget THEN toeic.Score
			ELSE 0 END
			) AS ToeicPolicyScore
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN 1
			ELSE 0 END
			) AS HasDisciplineScore
		,0 AS DisciplineScore
		,epa.[Status] AS StatusId
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.Email
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,period.PeriodId
		,period.[Name]
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
	FROM @TblEPAId AS tbl
		INNER JOIN dbo.EPA AS epa WITH (NOLOCK) ON tbl.Id = epa.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN NexHRM.dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON epa.EmployeeId = toeic.EmployeeId
	ORDER BY emp.NickName

	-- Process
	EXECUTE dbo.EPA_GetProcess @TblEPAId
END