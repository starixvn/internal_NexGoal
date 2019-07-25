CREATE PROCEDURE [dbo].[Probation_InsertNewEmployees]
	@EmployeeIds VARCHAR(MAX)
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

	SELECT val AS EmployeeId
		,e.JoinedDate
		,e.GroupId
	INTO #newemployee
	FROM dbo.func_SplitString(@EmployeeIds, ',') se
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON se.val = e.EmployeeId

	INSERT INTO [dbo].[ProbationEvaluation]
           ([EmployeeId]
           ,[CreatedDate])
	SELECT EmployeeId
		,@Today
	FROM #newemployee

	INSERT INTO dbo.ProbationSetting (EmployeeId, ProcessId, EvaluatorId, FromDate, ToDate)
	SELECT e.EmployeeId
		,@SelfProcessId
		,e.EmployeeId
		,DATEADD(DAY, -9,  DATEADD(MONTH, 2, e.JoinedDate))
		,DATEADD(DAY, -6, DATEADD(MONTH, 2, e.JoinedDate))
	FROM #newemployee e WITH (NOLOCK)

	INSERT INTO dbo.ProbationSetting (EmployeeId, ProcessId, EvaluatorId, FromDate, ToDate)
	SELECT e.EmployeeId
		,gfs.ProcessId
		,gfs.EvaluatorId
		,CASE 
			WHEN gfs.ProcessId = @EvaluatorProcessId THEN e.JoinedDate
			WHEN gfs.ProcessId = @ReviewerProcessId THEN DATEADD(DAY, -3, DATEADD(MONTH, 2, e.JoinedDate))
		END
		,CASE 
			WHEN gfs.ProcessId = @EvaluatorProcessId THEN DATEADD(DAY, -4, DATEADD(MONTH, 2, e.JoinedDate))
			WHEN gfs.ProcessId = @ReviewerProcessId THEN DATEADD(DAY, -3, DATEADD(MONTH, 2, e.JoinedDate))
		END
	FROM #newemployee e WITH (NOLOCK)
		INNER JOIN dbo.GroupFlowSetting gfs WITH (NOLOCK) ON e.GroupId = gfs.GroupId
	WHERE gfs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)

	INSERT INTO dbo.ProbationEvaluationDetail(EmployeeId, KPIId)
	SELECT ne.EmployeeId
		,k.KPIId
	FROM dbo.#newemployee ne, dbo.KPI k WITH (NOLOCK)

	DROP TABLE #newemployee
END