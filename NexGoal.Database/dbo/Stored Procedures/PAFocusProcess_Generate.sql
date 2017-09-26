




CREATE PROCEDURE [dbo].[PAFocusProcess_Generate] @PeriodId INT
	,@EmployeeId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160830@Razor.Nguyen
Task:PAFocus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT INTO dbo.PAFocusProcess
		SELECT d.PAFocusId
			,pSetting.EvaluatorId
			,setting.ProcessId
			,p.PAFocusStartDate
			,DATEADD(day, setting.Duration - 1, p.PAFocusStartDate)
		FROM dbo.PAFocus AS d WITH (NOLOCK)
			INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = d.PeriodId
			INNER JOIN dbo.PeriodFlowSetting AS pSetting WITH (NOLOCK) ON pSetting.PeriodId = d.PeriodId
			INNER JOIN dbo.PAFocusProcessSetting AS setting WITH (NOLOCK) ON setting.PeriodId = p.PeriodId
			AND setting.ProcessId = pSetting.ProcessId
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = d.EmployeeId
		WHERE d.PeriodId = @PeriodId
			AND d.EmployeeId = @EmployeeId
			AND setting.ProcessId = 3
			AND pSetting.EmployeeId = @EmployeeId

		INSERT INTO dbo.PAFocusProcess
		SELECT d.PAFocusId
			,pSetting.EvaluatorId
			,psetting.ProcessId
			,DATEADD(day, 1, p.PAFocusEndDate) AS StartDate
			,CONVERT(DATETIME, '9999-12-31') AS EndDate
		FROM dbo.PAFocus AS d WITH (NOLOCK)
			INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = d.PeriodId
			INNER JOIN dbo.PeriodFlowSetting AS pSetting WITH (NOLOCK) ON pSetting.PeriodId = d.PeriodId			
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = d.EmployeeId			
		WHERE d.PeriodId = @PeriodId
			AND d.EmployeeId = @EmployeeId
			AND psetting.ProcessId = 4
			AND pSetting.EmployeeId = @EmployeeId

	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = ERROR_MESSAGE()
	END CATCH
END