
CREATE PROCEDURE [dbo].[EmployeeFlowSetting_Delete] @EmployeeId INT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180808@Tim.Trinh
Task: delete employee flow setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX) = NULL
		,@xml XML

	SET @xml = (
			SELECT @EmployeeId AS '@empid'
				,(
					SELECT ProcessId AS '@pid'
						,EvaluatorId AS '@evaid'
						,[Priority] AS '@p'
					FROM dbo.EmployeeFlowSetting WITH (NOLOCK)
					WHERE EmployeeId = @EmployeeId
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE
	FROM dbo.EmployeeFlowSetting
	WHERE EmployeeId = @EmployeeId

	-- write log after delete
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'EmployeeFlowSetting_Delete'
		,@ObjectId = @EmployeeId
		,@OldData = @logDataOld
		,@Duration = @Duration
END
