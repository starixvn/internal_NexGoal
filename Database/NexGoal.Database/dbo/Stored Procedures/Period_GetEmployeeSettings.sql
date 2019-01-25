
CREATE PROCEDURE [dbo].[Period_GetEmployeeSettings] @PeriodId INT
	,@GroupId INT = NULL
	,@IsEvaluated BIT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20180823@Elton.Ton
Task: get employee settings
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PARecordTbl TABLE (Id INT PRIMARY KEY);

	INSERT INTO @PARecordTbl
	SELECT par.PARecordId
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
	WHERE par.PeriodId = @PeriodId
		AND (
			@StatusId IS NULL
			OR par.StatusId = @StatusId
			)
		AND (
			@GroupId IS NULL
			OR g.GroupId = @GroupId
			)
		AND (
			@IsEvaluated IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.PAProcess AS pap WITH (NOLOCK)
					INNER JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON pap.PerformanceRecordId = pr.PerformanceRecordId
					AND pap.PARecordId = par.PARecordId
				)
			)

	SELECT par.PARecordId
		,par.PeriodId
		,e.EmployeeId
		,par.StatusId
		,rr.Value AS [Status]
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON par.StatusId = rr.ResourceId
		AND rr.RefName = 'PerformanceStatus'
		INNER JOIN @PARecordTbl AS tbl ON par.PARecordId = tbl.Id
	ORDER BY par.PARecordId

	SELECT pap.PARecordId
		,pap.ProcessId
		,rr.Value AS Process
		,pap.EvaluatorId
		,pap.StartDate
		,pap.EndDate
	FROM dbo.PAProcess AS pap WITH (NOLOCK)
		INNER JOIN @PARecordTbl AS tbl ON pap.PARecordId = tbl.Id
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON pap.ProcessId = rr.ResourceId
		AND rr.RefName = 'PerformanceProcess'
	WHERE pap.ProcessId <> 2
		AND pap.ProcessId <> 3
	ORDER BY pap.PARecordId
		,pap.ProcessId
END
