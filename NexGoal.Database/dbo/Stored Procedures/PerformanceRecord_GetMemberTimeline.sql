
CREATE PROCEDURE [dbo].[PerformanceRecord_GetMemberTimeline] @ActorId INT
	,@PeriodId INT = NULL
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

	DECLARE @FromDate DATE
		,@ToDate DATE
		,@PAClosed TINYINT = 4
	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5
	-- Performance Record Status
	DECLARE @Submitted TINYINT = 3
		,@Closed TINYINT = 4

	CREATE TABLE #tblMember (
		EmployeeId INT
		,IsHost BIT
		,[Priority] TINYINT
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
		,NominationStatusId TINYINT
		,NominationPeriodId INT
		,TypeId TINYINT
		)

	CREATE TABLE #tblPARecord (
		RowNum INT
		,EmployeeId INT
		,GradeId VARCHAR(MAX)
		)

	IF @PeriodId IS NOT NULL
	BEGIN
		SELECT @FromDate = DisciplineStartDate
			,@ToDate = DisciplineEndDate
		FROM dbo.Period WITH (NOLOCK)
		WHERE PeriodId = @PeriodId
	END

	INSERT INTO #tblMember (
		EmployeeId
		,IsHost
		,[Priority]
		)
	SELECT data.EmployeeId
		,MAX(data.IsHost)
		,MIN(data.[Priority])
	FROM (
		SELECT EmployeeId
			,(CASE 
				WHEN ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId) THEN 1
				ELSE 0 END
				) AS IsHost
			,(CASE 
				WHEN ProcessId = @EvaluatorProcessId THEN 1
				WHEN ProcessId = @ReviewerProcessId THEN 2
				ELSE 3 END
				) AS [Priority]
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
		,NominationPeriodId
		,NominationStatusId
		,TypeId
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
		,nomination.EOQYId AS NominationPeriodId
		,nomination.StatusId AS NominationStatusId
		,record.TypeId
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		AND creator.[Enabled] = 1
		AND creator.IsDeleted = 0
		LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		AND nomination.NomineeId = @EmployeeId
		AND nomination.StatusId <> 4 -- draft
	WHERE (
			record.TypeId = @Checkin
			OR (
				record.TypeId IN (@SystemPA, @SystemNomination)
				AND @EmployeeId IS NOT NULL
				)
			)
		AND record.StatusId IN (@Submitted, @Closed)
		AND (
			record.CreatorId = - 1
			OR creator.EmployeeId IS NOT NULL
			)
		AND (
			member.IsHost = 1
			OR (
				member.IsHost = 0
				AND record.CreatorId = @ActorId
				)
			)
		AND (
			@PeriodId IS NULL
			OR record.CreatedDate BETWEEN @FromDate
				AND @ToDate
			)

	INSERT INTO #tblPARecord (
		RowNum
		,EmployeeId
		,GradeId
		)
	SELECT ROW_NUMBER() OVER (
			PARTITION BY pa.EmployeeId ORDER BY pa.EmployeeId
				,process.EndDate DESC
			) AS RowNum
		,pa.EmployeeId
		,pa.GradeId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @ReviewerProcessId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON pa.EmployeeId = member.EmployeeId
		AND member.IsHost = 1
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
	WHERE pa.StatusId = @PAClosed

	SELECT emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,pa.GradeId
		,record.TotalItems
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		LEFT JOIN #tblPARecord AS pa WITH (NOLOCK) ON emp.EmployeeId = pa.EmployeeId
		AND pa.RowNum = 1
		LEFT JOIN (
		SELECT EmployeeIds
			,COUNT(1) AS TotalItems
		FROM #tblPerformanceRecord WITH (NOLOCK)
		GROUP BY EmployeeIds
		) AS record ON emp.EmployeeId = record.EmployeeIds
	ORDER BY member.[Priority]
		,emp.NickName

	IF @EmployeeId IS NULL
	BEGIN
		SELECT PerformanceRecordId
			,EmployeeIds
			,CreatorId
			,GradeId
			,StatusId
			,RoleId
			,CreatedDate
			,NominationPeriodId
			,NominationStatusId
			,TypeId
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
			,NominationPeriodId
			,NominationStatusId
			,TypeId
		FROM #tblPerformanceRecord WITH (NOLOCK)
		ORDER BY CreatedDate DESC OFFSET(@PageIndex * @PageSize) ROWS

		FETCH NEXT @PageSize ROWS ONLY
	END

	SELECT period.EOQYId AS NominationPeriodId
		,period.[Quarter]
		,period.[Year]
	FROM #tblPerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecord

	DROP TABLE #tblPARecord
END
