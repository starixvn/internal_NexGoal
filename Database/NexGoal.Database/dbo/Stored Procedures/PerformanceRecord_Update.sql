


CREATE PROCEDURE [dbo].[PerformanceRecord_Update] @PerformanceRecordIds VARCHAR(MAX) = '' OUTPUT
	,@PerformanceRecordId INT
	,@EmployeeIds VARCHAR(MAX) = ''
	,@CreatorId INT
	,@GradeId VARCHAR(MAX) = ''
	,@Comment NVARCHAR(MAX)
	,@ProjectIds VARCHAR(MAX) = NULL
	,@IsFromDraft BIT = 0
	,@TypeId TINYINT = 1
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180913@Tim.Trinh
Task: update performance record
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @RoleId TINYINT
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tblMember (
		RowNum INT IDENTITY(1, 1)
		,EmployeeId INT
		,RoleId TINYINT
		,[Priority] TINYINT
		)

	CREATE TABLE #tblPerformanceRecordOutput (
		RowNum INT IDENTITY(1, 1)
		,PerformanceRecordId INT
		)

	-- Write log before update
	IF(@IsFromDraft = 1)
	BEGIN
			SET @xml = (
				SELECT DraftPerformanceRecordId AS '@ids'
					,EmployeeIds AS '@eids'
					,CreatorId AS '@cid'
					,GradeId AS '@gid'
					,Comment AS '@comment'
					,ProjectIds AS '@pids'
					,ModifiedDate AS '@mdate'
					,TypeId AS '@type'
					,RoleId AS '@role'
				FROM dbo.DraftPerformanceRecord WITH (NOLOCK)
				WHERE DraftPerformanceRecordId = @PerformanceRecordId
				FOR XML PATH('r')
					,TYPE
				)
	END
	ELSE
	BEGIN
		SET @xml = (
				SELECT PerformanceRecordId AS '@ids'
					,EmployeeId AS '@eid'
					,CreatorId AS '@cid'
					,GradeId AS '@gid'
					,Comment AS '@comment'
					,ProjectIds AS '@pids'
					,ModifiedDate AS '@mdate'
					,TypeId AS '@type'
					,RoleId AS '@role'
				FROM dbo.PerformanceRecord WITH (NOLOCK)
				WHERE PerformanceRecordId = @PerformanceRecordId
				FOR XML PATH('r')
					,TYPE
				)
	END
	
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	
	INSERT INTO #tblMember (
		EmployeeId
		,RoleId
		,[Priority]
		)
	SELECT DISTINCT val
		,@SelfProcessId
		,0
	FROM dbo.func_SplitString(@EmployeeIds, ',')
	WHERE ISNUMERIC(val) = 1

	UPDATE tbl
	SET tbl.RoleId = CASE 
			WHEN r.[Priority] = 3 THEN @EvaluatorProcessId
			WHEN r.[Priority] = 2 THEN @ReviewerProcessId
			ELSE @PMProcessId
			END
		,tbl.[Priority] = r.[Priority]
	FROM #tblMember AS tbl WITH (ROWLOCK, UPDLOCK)
		INNER JOIN (
		SELECT data.EmployeeId
			,MAX(data.[Priority]) AS [Priority]
		FROM (
			SELECT EmployeeId
				,(CASE 
					WHEN ProcessId = @EvaluatorProcessId THEN 3
					WHEN ProcessId = @ReviewerProcessId THEN 2
					ELSE 1 END
					) AS [Priority]
			FROM dbo.func_GetMember(@CreatorId)
			WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
			) AS data
		GROUP BY data.EmployeeId
		) AS r ON tbl.EmployeeId = r.EmployeeId

	IF @IsFromDraft = 0
	BEGIN
		SELECT TOP 1 @RoleId = RoleId
		FROM #tblMember WITH (NOLOCK)
		ORDER BY [Priority] DESC

		UPDATE dbo.PerformanceRecord WITH (ROWLOCK, UPDLOCK)
		SET EmployeeId = @EmployeeIds
			,CreatorId = @CreatorId
			,GradeId = @GradeId
			,Comment = @Comment
			,ProjectIds = @ProjectIds
			,TypeId = @TypeId
			,RoleId = @RoleId
			,ModifiedDate = GETDATE()
		WHERE PerformanceRecordId = @PerformanceRecordId

		SELECT @PerformanceRecordIds = CAST(@PerformanceRecordId AS VARCHAR(20))
	END
	ELSE
	BEGIN
		DELETE FROM dbo.DraftPerformanceRecord WHERE DraftPerformanceRecordId = @PerformanceRecordId

		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,GradeId
			,Comment
			,ProjectIds
			,CreatedDate
			,TypeId
			,RoleId
			)
		OUTPUT INSERTED.PerformanceRecordId
		INTO #tblPerformanceRecordOutput(PerformanceRecordId)
		SELECT EmployeeId
			,@CreatorId
			,@GradeId
			,@Comment
			,@ProjectIds
			,GETDATE()
			,@TypeId
			,RoleId
		FROM #tblMember WITH (NOLOCK)

		SELECT @PerformanceRecordIds = COALESCE(@PerformanceRecordIds + ',', '') + CAST(PerformanceRecordId AS VARCHAR(20))
		FROM #tblPerformanceRecordOutput WITH (NOLOCK)
	END

	-- write log after update
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordIds AS '@ids'
				,@EmployeeIds AS '@eids'
				,@CreatorId AS '@cid'
				,@GradeId AS '@gid'
				,@Comment AS '@comment'
				,@ProjectIds AS '@pids'
				,GETDATE() AS '@mdate'
				,@TypeId AS '@type'
				,@RoleId AS '@role'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PerformanceRecord_Update'
		,@OldData = @logDataOld
		,@Data = @logData
		,@Duration = @duration

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecordOutput
END
