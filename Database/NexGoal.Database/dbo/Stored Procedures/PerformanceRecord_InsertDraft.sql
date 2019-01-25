

CREATE PROCEDURE [dbo].[PerformanceRecord_InsertDraft] @PerformanceRecordIds VARCHAR(MAX) = '' OUTPUT
	,@EmployeeIds VARCHAR(MAX) = ''
	,@CreatorId INT
	,@GradeId VARCHAR(MAX) = ''
	,@Comment NVARCHAR(MAX)
	,@ProjectIds VARCHAR(MAX) = NULL
	,@TypeId TINYINT = 1
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180824@Tim.Trinh
Task: insert draft performance record
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

	INSERT INTO #tblMember (
		EmployeeId
		,RoleId
		,[Priority]
		)
	SELECT DISTINCT val
		,CASE 
			WHEN val = @CreatorId THEN @SelfProcessId
			ELSE 0
			END
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
	
	SELECT TOP 1 @RoleId = RoleId
	FROM #tblMember WITH (NOLOCK)
	ORDER BY [Priority] DESC

	INSERT INTO dbo.DraftPerformanceRecord (
		EmployeeIds
		,CreatorId
		,GradeId
		,Comment
		,ProjectIds
		,CreatedDate
		,TypeId
		,RoleId
		)
	OUTPUT INSERTED.DraftPerformanceRecordId
	INTO #tblPerformanceRecordOutput(PerformanceRecordId)
	VALUES (
		@EmployeeIds
		,@CreatorId
		,@GradeId
		,@Comment
		,@ProjectIds
		,GETDATE()
		,@TypeId
		,@RoleId
		)

	SELECT @PerformanceRecordIds = COALESCE(@PerformanceRecordIds + ',', '') + CAST(PerformanceRecordId AS VARCHAR(20))
	FROM #tblPerformanceRecordOutput

	-- write log after insert
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordIds AS '@id'
				,@EmployeeIds AS '@eids'
				,@CreatorId AS '@cid'
				,@GradeId AS '@gid'
				,@Comment AS '@comment'
				,@ProjectIds AS '@pids'
				,@TypeId AS '@type'
				,@RoleId AS '@role'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PerformanceRecord_InsertDraft'
		,@ObjectId = @PerformanceRecordIds
		,@Data = @logData
		,@Duration = @duration

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecordOutput
END