
CREATE PROCEDURE [dbo].[PerformanceRecord_Update] @PerformanceRecordId INT
	,@EmployeeIds VARCHAR(MAX) = ''
	,@CreatorId INT
	,@GradeId VARCHAR(MAX) = ''
	,@Comment NVARCHAR(MAX)
	,@ProjectIds VARCHAR(MAX) = NULL
	,@StatusId VARCHAR(MAX)
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

	DECLARE @RoleId TINYINT
		,@Draft TINYINT = 2
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
	SET @xml = (
			SELECT PerformanceRecordId AS '@id'
				,EmployeeIds AS '@eids'
				,CreatorId AS '@cid'
				,GradeId AS '@gid'
				,Comment AS '@comment'
				,StatusId AS '@sid'
				,ProjectIds AS '@pids'
				,CreatedDate AS '@cdate'
				,TypeId AS '@type'
			FROM dbo.PerformanceRecord WITH (NOLOCK)
			WHERE PerformanceRecordId = @PerformanceRecordId
			FOR XML PATH('r')
				,TYPE
			)
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

	IF @StatusId = @Draft
	BEGIN
		SELECT TOP 1 @RoleId = RoleId
		FROM #tblMember WITH (NOLOCK)
		ORDER BY [Priority] DESC
		
		UPDATE dbo.PerformanceRecord WITH (ROWLOCK, UPDLOCK)
		SET EmployeeIds = @EmployeeIds
			,CreatorId = @CreatorId
			,GradeId = @GradeId
			,Comment = @Comment
			,StatusId = @StatusId
			,ProjectIds = @ProjectIds
			,ModifiedDate = GETDATE()
			,TypeId = @TypeId
			,RoleId = @RoleId
		WHERE PerformanceRecordId = @PerformanceRecordId
	END
	ELSE
	BEGIN
		UPDATE record
		SET record.EmployeeIds = member.EmployeeId
			,record.CreatorId = @CreatorId
			,record.GradeId = @GradeId
			,record.Comment = @Comment
			,record.StatusId = @StatusId
			,record.ProjectIds = @ProjectIds
			,record.ModifiedDate = GETDATE()
			,record.TypeId = @TypeId
			,record.RoleId = member.RoleId
		FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
			,#tblMember AS member WITH (NOLOCK)
		WHERE record.PerformanceRecordId = @PerformanceRecordId
			AND member.RowNum = 1

		INSERT INTO dbo.PerformanceRecord (
			EmployeeIds
			,CreatorId
			,GradeId
			,Comment
			,StatusId
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
			,@StatusId
			,@ProjectIds
			,GETDATE()
			,@TypeId
			,RoleId
		FROM #tblMember WITH (NOLOCK)
		WHERE RowNum > 1
	END

	INSERT INTO #tblPerformanceRecordOutput (PerformanceRecordId)
	VALUES (@PerformanceRecordId)

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to update performance.'

		RETURN
	END

	-- write log after insert
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	DECLARE @totalRecords INT
		,@rowNum INT = 1
		,@recordId INT

	SELECT @totalRecords = COUNT(1)
	FROM #tblPerformanceRecordOutput WITH (NOLOCK)

	WHILE @rowNum <= @totalRecords
	BEGIN
		SELECT @recordId = PerformanceRecordId
		FROM #tblPerformanceRecordOutput WITH (NOLOCK)
		WHERE RowNum = @rowNum

		SET @xml = (
				SELECT PerformanceRecordId AS '@id'
					,EmployeeIds AS '@eids'
					,CreatorId AS '@cid'
					,GradeId AS '@gid'
					,Comment AS '@comment'
					,StatusId AS '@sid'
					,ProjectIds AS '@pids'
					,CreatedDate AS '@cdate'
					,TypeId AS '@tid'
					,RoleId AS '@rid'
				FROM dbo.PerformanceRecord WITH (NOLOCK)
				WHERE PerformanceRecordId = @recordId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
			,@SPName = 'PerformanceRecord_Update'
			,@ObjectId = @recordId
			,@Data = @logData
			,@Duration = @Duration

		SET @rowNum = @rowNum + 1
	END

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecordOutput
END