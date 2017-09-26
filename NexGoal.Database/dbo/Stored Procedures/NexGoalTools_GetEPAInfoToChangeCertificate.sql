

CREATE PROCEDURE [dbo].[NexGoalTools_GetEPAInfoToChangeCertificate] @EPAIdFrom INT
	,@EPAIdTo INT
AS
/*
Created: 20160829@Tim.Trinh
Task: Get EPA info to change certificate.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ToDay DATE

	SET @ToDay = CAST(GETDATE() AS DATE)

	-- General Info
	SELECT DISTINCT epa.EPAId
		,epa.FinalScore
		,epa.Grade
		,epa.[Status] AS StatusId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.ScoreRatingType AS srt WITH (NOLOCK) ON epa.ScoreRatingTypeId = srt.ScoreRatingTypeId
		INNER JOIN dbo.TargetType AS tt WITH (NOLOCK) ON period.TargetTypeId = tt.TargetTypeId
	WHERE epa.EPAId >= @EPAIdFrom
		AND epa.EPAId < @EPAIdTo

	-- Process
	SELECT DISTINCT epa.EPAId
		,pc.ProcessId
		,pc.EvaluatorId
		,pc.TotalScore
		,pc.AdjustScore
		,(CASE 
			WHEN CAST(pc.StartDate AS DATE) > @ToDay THEN 2
			WHEN @ToDay BETWEEN CAST(pc.StartDate AS DATE)
					AND CAST(pc.EndDate AS DATE) THEN 0
			WHEN CAST(pc.EndDate AS DATE) > @ToDay THEN 1 END
			) AS StatusId -- 0: in progress; 1: closed; 2: pending
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId > 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pc.EvaluatorId = emp.EmployeeId
	WHERE epa.EPAId >= @EPAIdFrom
		AND epa.EPAId < @EPAIdTo
	ORDER BY epa.EPAId
		,pc.ProcessId

	-- Detail
	SELECT DISTINCT epa.EPAId
		,dt.ProcessId
		,dt.EvaluatorId
		,dt.KPIGroupId
		,dt.KPIId
		,dt.Score
		,dt.Rate
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPADetail AS dt WITH (NOLOCK) ON epa.EPAId = dt.EPAId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON dt.EvaluatorId = emp.EmployeeId
	WHERE epa.EPAId >= @EPAIdFrom
		AND epa.EPAId < @EPAIdTo
	ORDER BY epa.EPAId
		,dt.ProcessId
END