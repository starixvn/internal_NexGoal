
CREATE PROCEDURE [dbo].[PerformanceRecord_GetMemberTimeline] @ActorId INT
	,@EmployeeId INT = NULL
	,@PageIndex INT = 0
	,@PageSize INT = 4
AS
/*
Created: 20181024@Tim.Trinh
Task: Get member timeline.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
	-- Performance Record Status
	DECLARE @Submitted TINYINT = 3
		,@Closed TINYINT = 4

	CREATE TABLE #tblMember (
		EmployeeId INT
		,IsHost BIT
		)

	CREATE TABLE #tblPerformanceRecord (
		RowNum INT
		,PerformanceRecordId INT
		,EmployeeIds VARCHAR(MAX)
		,CreatorId INT
		,GradeId VARCHAR(MAX)
		,StatusId TINYINT
		,RoleId TINYINT
		,CreatedDate DATETIME
		)

	INSERT INTO #tblMember (
		EmployeeId
		,IsHost
		)
	SELECT data.EmployeeId
		,MAX(data.IsHost)
	FROM (
		SELECT EmployeeId
			,(CASE 
				WHEN ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId) THEN 1
				ELSE 0 END
				) AS IsHost
		FROM dbo.func_GetMember(@ActorId)
		WHERE ISNULL(@EmployeeId, EmployeeId) = EmployeeId
			AND ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		) AS data
	GROUP BY data.EmployeeId

	INSERT INTO #tblPerformanceRecord (
		RowNum
		,PerformanceRecordId
		,EmployeeIds
		,CreatorId
		,GradeId
		,StatusId
		,RoleId
		,CreatedDate
		)
	SELECT ROW_NUMBER() OVER (
			PARTITION BY record.EmployeeIds ORDER BY record.EmployeeIds
				,record.CreatedDate DESC
			) AS RowNum
		,record.PerformanceRecordId
		,record.EmployeeIds
		,record.CreatorId
		,record.GradeId
		,(CASE 
			WHEN record.CreatorId <> @ActorId THEN @Closed
			ELSE record.StatusId END
			) AS StatusId
		,record.RoleId
		,record.CreatedDate
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		AND creator.[Enabled] = 1
		AND creator.IsDeleted = 0
	WHERE record.TypeId = 1
		AND record.StatusId IN (@Submitted, @Closed)
		AND (
			member.IsHost = 1
			OR (
				member.IsHost = 0
				AND (
					record.CreatorId = @ActorId
					OR record.EmployeeIds = record.CreatorId
					)
				)
			)

	SELECT emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,record.TotalItems
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		LEFT JOIN (
		SELECT EmployeeIds
			,COUNT(1) AS TotalItems
		FROM #tblPerformanceRecord WITH (NOLOCK)
		GROUP BY EmployeeIds
		) AS record ON emp.EmployeeId = record.EmployeeIds
	ORDER BY emp.NickName

	IF @EmployeeId IS NULL
	BEGIN
		SELECT PerformanceRecordId
			,EmployeeIds
			,CreatorId
			,GradeId
			,StatusId
			,RoleId
			,CreatedDate
		FROM #tblPerformanceRecord WITH (NOLOCK)
		WHERE RowNum <= @PageSize
	END
	ELSE
	BEGIN
		SELECT PerformanceRecordId
			,EmployeeIds
			,CreatorId
			,GradeId
			,StatusId
			,RoleId
			,CreatedDate
		FROM #tblPerformanceRecord WITH (NOLOCK)
		ORDER BY CreatedDate DESC OFFSET(@PageIndex * @PageSize) ROWS

		FETCH NEXT @PageSize ROWS ONLY
	END

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecord
END
