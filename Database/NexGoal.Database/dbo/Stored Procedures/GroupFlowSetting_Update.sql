
CREATE PROCEDURE [dbo].[GroupFlowSetting_Update] @GroupId INT
	,@FlowDetails NVARCHAR(MAX)
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180802@Tim.Trinh
Task: update group flow setting
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
	SET XACT_ABORT ON;

	DECLARE @idoc INT
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX) = NULL
		,@logDataOld NVARCHAR(MAX) = NULL
		,@xml XML

	CREATE TABLE #tblGroupFlowSetting (
		ProcessId TINYINT
		,EvaluatorId INT
		,[Priority] TINYINT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@FlowDetails

	INSERT INTO #tblGroupFlowSetting (
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
			FROM dbo.GroupFlowSetting WITH (NOLOCK)
			WHERE GroupId = @GroupId
			)
	BEGIN
		-- Write log before update	
		SET @xml = (
				SELECT @GroupId AS '@gid'
					,(
						SELECT ProcessId AS '@pid'
							,EvaluatorId AS '@evaid'
							,[Priority] AS '@p'
						FROM dbo.GroupFlowSetting WITH (NOLOCK)
						WHERE GroupId = @GroupId
						FOR XML PATH('z')
							,TYPE
						)
				FOR XML PATH('r')
					,TYPE
				)
		SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		DELETE
		FROM dbo.GroupFlowSetting
		WHERE GroupId = @GroupId
	END

	INSERT INTO dbo.GroupFlowSetting (
		GroupId
		,ProcessId
		,EvaluatorId
		,[Priority]
		)
	SELECT @GroupId
		,ProcessId
		,EvaluatorId
		,[Priority]
	FROM #tblGroupFlowSetting WITH (NOLOCK)

	-- write log after update
	SET @xml = (
			SELECT @GroupId AS '@gid'
				,(
					SELECT ProcessId AS '@pid'
						,EvaluatorId AS '@evaid'
						,[Priority] AS '@p'
					FROM dbo.GroupFlowSetting WITH (NOLOCK)
					WHERE GroupId = @GroupId
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
		,@SPName = 'GroupFlowSetting_Update'
		,@ObjectId = @GroupId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #tblGroupFlowSetting
END
