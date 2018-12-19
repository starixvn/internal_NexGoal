
CREATE PROCEDURE [dbo].[EmployeeFlowSetting_Update] @EmployeeId INT
	,@FlowDetails NVARCHAR(MAX)
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180808@Tim.Trinh
Task: update employee flow setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
@FlowDetails: 
    N'<ROOT>
	   <Process ProcessId="1" EvaluatorId="125" Priority="1" />
    </ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @idoc INT
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX) = NULL
		,@logDataOld NVARCHAR(MAX) = NULL
		,@xml XML

	CREATE TABLE #tblEmployeeFlowSetting (
		ProcessId TINYINT
		,EvaluatorId INT
		,[Priority] TINYINT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@FlowDetails

	INSERT INTO #tblEmployeeFlowSetting (
		ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT ProcessId
		,EvaluatorId
		,[Priority]
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (ProcessId TINYINT, EvaluatorId INT, [Priority] TINYINT) AS data

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeFlowSetting WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			)
	BEGIN
		-- Write log before update	
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
	END

	INSERT INTO dbo.EmployeeFlowSetting (
		EmployeeId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT @EmployeeId
		,ProcessId
		,EvaluatorId
		,[Priority]
	FROM #tblEmployeeFlowSetting WITH (NOLOCK)

	-- write log after update
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
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'EmployeeFlowSetting_Update'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #tblEmployeeFlowSetting
END
