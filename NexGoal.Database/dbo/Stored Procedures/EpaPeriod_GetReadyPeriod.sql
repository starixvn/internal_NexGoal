
CREATE PROCEDURE [dbo].[EpaPeriod_GetReadyPeriod] @TargetTypeId INT = NULL
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Get all evaluation period.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEmployeeSetting (
		PeriodId INT
		,EmployeeId INT
		)

	INSERT INTO #tblEmployeeSetting (
		PeriodId
		,EmployeeId
		)
	SELECT DISTINCT period.PeriodId
		,peFS.EmployeeId
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PeriodFlowSetting AS peFS WITH (NOLOCK) ON period.PeriodId = peFS.PeriodId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
			WHERE peFS.EmployeeId = peExcluded.EmployeeId
				AND peFS.PeriodId = peExcluded.PeriodId
			)
		AND ISNULL(@TargetTypeId, period.TargetTypeId) = period.TargetTypeId
	
	UNION
	SELECT period.PeriodId
		,peTS.EmployeeId
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PeriodTargetSetting AS peTS WITH (NOLOCK) ON period.PeriodId = peTS.PeriodId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodFlowSetting AS peFS WITH (NOLOCK)
			WHERE peTS.EmployeeId = peFS.EmployeeId
				AND peTS.PeriodId = peFS.PeriodId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PeriodExcludedEmployee AS peExcluded WITH (NOLOCK)
			WHERE peTS.EmployeeId = peExcluded.EmployeeId
				AND peTS.PeriodId = peExcluded.PeriodId
			)
		AND ISNULL(@TargetTypeId, period.TargetTypeId) = period.TargetTypeId

	SELECT DISTINCT pe.PeriodId
		,pe.[Name]
		,pe.StartDate
		,pe.EndDate
		,pe.AttendanceStartDate
		,pe.AttendanceEndDate
		,pe.PAFocusStartDate
		,pe.[Description]
		,pe.IsActive
		,peF.TotalEmployees
		,epa.TotalEmployeesUnfinished
		,tar.TargetTypeId
		,tar.[Name]
	FROM dbo.Period AS pe WITH (NOLOCK)
		INNER JOIN dbo.TargetType AS tar WITH (NOLOCK) ON pe.TargetTypeId = tar.TargetTypeId
		LEFT JOIN (
		SELECT PeriodId
			,COUNT(EmployeeId) AS TotalEmployees
		FROM #tblEmployeeSetting WITH (NOLOCK)
		GROUP BY PeriodId
		) AS peF ON peF.PeriodId = pe.PeriodId
		LEFT JOIN (
		SELECT PeriodId
			,COUNT(EmployeeId) AS TotalEmployeesUnfinished
		FROM dbo.EPA WITH (NOLOCK)
		WHERE [Status] = 0
		GROUP BY PeriodId
		) AS epa ON pe.PeriodId = epa.PeriodId
	WHERE ISNULL(@TargetTypeId, pe.TargetTypeId) = pe.TargetTypeId
		AND (
			pe.StartDate IS NULL
			OR pe.StartDate <= GETDATE()
			)
		AND pe.IsActive = 1
	ORDER BY pe.EndDate DESC

	DROP TABLE #tblEmployeeSetting
END