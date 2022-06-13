
CREATE PROCEDURE [dbo].[Probation_InsertNewEmployees] @EmployeeIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190520@Elton.ton
Task: add new employees to probation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@SelfProcessId TINYINT = 2
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	DECLARE @TargetTemplateId INT

	SELECT TOP 1 @TargetTemplateId = TargetTemplateId
	FROM dbo.TargetTemplate WITH (NOLOCK)
	WHERE TargetTypeId = 1 -- probation
		AND IsActive = 1

	SELECT emp.EmployeeId
		,emp.JoinedDate
		,emp.GroupId
		,emp.NickName
	INTO #newemployee
	FROM dbo.func_SplitString(@EmployeeIds, ',') se
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON se.val = emp.EmployeeId

	SELECT EmployeeId
		,JoinedDate
		,GroupId
	INTO #employeeHasSetting
	FROM #newemployee AS emp WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
			WHERE gfs.GroupId = emp.GroupId
			)

	INSERT INTO dbo.ProbationEvaluation (
		EmployeeId
		,CreatedDate
		)
	SELECT EmployeeId
		,@Today
	FROM #employeeHasSetting WITH (NOLOCK)

	INSERT INTO dbo.ProbationSetting (
		EmployeeId
		,ProcessId
		,EvaluatorId
		,FromDate
		,ToDate
		)
	SELECT EmployeeId
		,@SelfProcessId
		,EmployeeId
		,DATEADD(DAY, - 9, DATEADD(MONTH, 2, JoinedDate))
		,DATEADD(DAY, - 6, DATEADD(MONTH, 2, JoinedDate))
	FROM #employeeHasSetting WITH (NOLOCK)

	INSERT INTO dbo.ProbationSetting (
		EmployeeId
		,ProcessId
		,EvaluatorId
		,FromDate
		,ToDate
		)
	SELECT emp.EmployeeId
		,gfs.ProcessId
		,gfs.EvaluatorId
		,CASE 
			WHEN gfs.ProcessId = @EvaluatorProcessId THEN @Today
			WHEN gfs.ProcessId = @ReviewerProcessId THEN DATEADD(DAY, - 3, DATEADD(MONTH, 2, emp.JoinedDate))
			END
		,CASE 
			WHEN gfs.ProcessId = @EvaluatorProcessId THEN DATEADD(DAY, - 4, DATEADD(MONTH, 2, emp.JoinedDate))
			WHEN gfs.ProcessId = @ReviewerProcessId THEN DATEADD(DAY, - 3, DATEADD(MONTH, 2, emp.JoinedDate))
			END
	FROM #employeeHasSetting AS emp WITH (NOLOCK)
		INNER JOIN dbo.GroupFlowSetting AS gfs WITH (NOLOCK) ON emp.GroupId = gfs.GroupId
	WHERE gfs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)

	INSERT INTO dbo.ProbationEvaluationDetail (
		EmployeeId
		,KPIId
		)
	SELECT emp.EmployeeId
		,k.KPIId
	FROM #employeeHasSetting AS emp WITH (NOLOCK)
		,dbo.KPI AS k WITH (NOLOCK)
		INNER JOIN dbo.TargetTemplateKPI AS ttk ON ttk.KPIGroupId = k.KPIGroupId
		AND k.KPIId = ttk.KPIId
	WHERE ttk.TargetTemplateId = @TargetTemplateId

	IF EXISTS (
			SELECT 1
			FROM #newemployee AS emp WITH (NOLOCK)
			WHERE NOT EXISTS (
					SELECT 1
					FROM #employeeHasSetting AS ehs WITH (NOLOCK)
					WHERE ehs.EmployeeId = emp.EmployeeId
					)
			)
	BEGIN
		SET @ErrCode = 1

		DECLARE @Employees NVARCHAR(MAX) = STUFF((
					SELECT CONCAT (', ', emp.NickName)
					FROM #newemployee AS emp WITH (NOLOCK)
					WHERE NOT EXISTS (
							SELECT 1
							FROM #employeeHasSetting AS ehs WITH (NOLOCK)
							WHERE ehs.EmployeeId = emp.EmployeeId
							)
					FOR XML PATH('')
					), 1, 2, '')

		SET @ErrMsg = 'These employees cant be added because they dont have flow setting: ' + @Employees
	END

	DROP TABLE #newemployee

	DROP TABLE #employeeHasSetting
END