
CREATE PROCEDURE [dbo].[Period_GetEmployeeSetting] @PARecordId INT
AS
/*
Created: 20180910@Elton.Ton
Task: get employee setting
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

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
	WHERE par.PARecordId = @PARecordId

	SELECT pap.PARecordId
		,pap.ProcessId
		,rr.Value AS Process
		,pap.EvaluatorId
		,pap.StartDate
		,pap.EndDate
	FROM dbo.PAProcess AS pap WITH (NOLOCK)
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON pap.ProcessId = rr.ResourceId
		AND rr.RefName = 'PerformanceProcess'
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON pap.EvaluatorId = e.EmployeeId
		LEFT JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON pr.PerformanceRecordId = pap.PerformanceRecordId
	WHERE pap.PARecordId = @PARecordId AND pap.ProcessId <> 2 AND pap.ProcessId <> 3
	ORDER BY pap.PARecordId
		,pap.ProcessId
END
