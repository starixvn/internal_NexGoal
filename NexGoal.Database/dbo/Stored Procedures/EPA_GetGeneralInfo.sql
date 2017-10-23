
CREATE PROCEDURE [dbo].[EPA_GetGeneralInfo] @TblEPAIds IntId READONLY
AS
/*
Created: 20150827@Tim.Trinh
Task: Get empoyee info by list epas.
DB: SMSEPA
Original: 

Revisions:  
    20171005@Tim.Trinh: get toeic policy score
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RequiredCommentOnMaxScore BIT
		,@Today DATE = GETDATE()
		,@ProbationTarget TINYINT = 1
		,@SemiTarget TINYINT = 3

	CREATE TABLE #tblLastEPA (
		EPAId INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,FinalScore NVARCHAR(MAX)
		,Grade NVARCHAR(MAX)
		,DisciplineScore FLOAT
		)

	SELECT @RequiredCommentOnMaxScore = CAST(lkd.Value AS BIT)
	FROM dbo.LookupData AS lkd WITH (NOLOCK)
		INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
		AND lk.Id = 7 -- required comment on max score

	INSERT INTO #tblLastEPA
	EXECUTE dbo.EPA_GetLastScore @TblEPAIds

	SELECT DISTINCT epa.EPAId
		,epa.FinalScore
		,epa.Grade
		,(CASE srt.ScoreRatingTypeId
			WHEN 1 THEN 1
			ELSE 0 END
			) AS HasDisciplineScore
		,0 AS DisciplineScore
		,srt.Attendance AS MaxDisciplineScore
		,ls.FinalScore AS LastFinalScore
		,ls.Grade AS LastGrade
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EPADetail AS dt WITH (NOLOCK)
					WHERE dt.EPAId = epa.EPAId
						AND dt.IsMaxScore = 1
					) THEN 1
			ELSE 0 END
			) AS HasMaxScoreOnKPI
		,@RequiredCommentOnMaxScore AS RequiredCommentOnMaxScore
		,(CASE 
			WHEN epa.[Status] = 1 THEN epa.ToeicPolicyScore
			WHEN tt.TargetTypeId = @SemiTarget THEN toeic.Score
			ELSE 0 END
			) AS ToeicPolicyScore
		,emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.JoinedDate
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.Id AS JobTitleId
		,job.[Name] AS JobTitleName
		,CAST(lkd.Value AS TINYINT) AS RatingTypeId
		,lkd.Code
		,lkd.[Name]
		,epa.[Status] AS StatusId
		,period.PeriodId
		,period.[Name]
		,CAST(pc.StartDate AS DATE) AS StartDate
		,(CASE 
			WHEN tt.TargetTypeId IN (@SemiTarget) THEN CASE 
					WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN CAST(pc.EndDate AS DATE)
					ELSE period.EndDate
					END
			ELSE CAST(pc.EndDate AS DATE) END
			) AS EndDate
		,(CASE 
			WHEN tt.TargetTypeId IN (@SemiTarget) THEN (CASE 
					WHEN CAST(pc.EndDate AS DATE) >= CAST(period.EndDate AS DATE) THEN (CASE 
							WHEN CAST(pc.EndDate AS DATE) < @Today
								AND epa.[Status] = 1 THEN 1
							ELSE 0 END
							)
					ELSE (CASE 
							WHEN CAST(period.EndDate AS DATE) <= @Today
								AND epa.[Status] = 1 THEN 1
							ELSE 0 END
							) END
					)
			ELSE (CASE 
					WHEN CAST(pc.EndDate AS DATE) < @Today
						AND epa.[Status] = 1 THEN 1
					ELSE 0 END
					) END
			) AS IsEndPeriod
		,tt.TargetTypeId
		,tt.[Name] AS TargetTypeName
		,srt.ScoreRatingTypeId
		,srt.[Name]
		,srt.SelfEvaluated
		,srt.Evaluator
		,srt.Reviewer
		,srt.Management
		,srt.Attendance
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN @TblEPAIds AS epaId ON epa.EPAId = epaId.Id
		INNER JOIN (
		SELECT pc1.EPAId
			,MIN(pc1.StartDate) AS StartDate
			,MAX(pc1.EndDate) AS EndDate
		FROM dbo.EPAProcess AS pc1 WITH (NOLOCK)
			INNER JOIN @TblEPAIds AS epaId1 ON pc1.EPAId = epaId1.Id
		GROUP BY pc1.EPAId
		) AS pc ON epa.EPAId = pc.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.[JobTitle] = job.[Id]
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.LookupData AS lkd WITH (NOLOCK) ON epa.RatingTypeId = CAST(lkd.Value AS TINYINT)
		INNER JOIN dbo.[Lookup] AS lk WITH (NOLOCK) ON lkd.LookupId = lk.Id
		AND lk.Id = 8 -- rating type
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
		LEFT JOIN #tblLastEPA AS ls WITH (NOLOCK) ON epa.EPAId = ls.EPAId
		AND emp.Code = ls.Code
		LEFT JOIN NexHRM.dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON epa.EmployeeId = toeic.EmployeeId
	ORDER BY emp.NickName
		,emp.Code

	DROP TABLE #tblLastEPA
END