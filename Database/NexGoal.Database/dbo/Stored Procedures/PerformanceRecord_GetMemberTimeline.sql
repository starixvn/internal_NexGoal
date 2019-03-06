
CREATE PROCEDURE [dbo].[PerformanceRecord_GetMemberTimeline] @ActorId INT
	,@PeriodId INT = NULL
	,@GroupIds VARCHAR(MAX) = NULL
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

	DECLARE @Today DATE = GETDATE()
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@PAClosed TINYINT = 4
	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5

	CREATE TABLE #tblGroup (GroupId INT)

	CREATE TABLE #tblMember (
		EmployeeId INT
		,IsHost BIT
		,[Priority] TINYINT
		)

	CREATE TABLE #tblPerformanceRecord (
		RowNum INT
		,PerformanceRecordId INT
		,EmployeeId INT
		,CreatorId INT
		,GradeId VARCHAR(MAX)
		,StatusId TINYINT
		,RoleId TINYINT
		,CreatedDate DATETIME
		,NominationStatusId TINYINT
		,NominationPeriodId INT
		,TypeId TINYINT
		,[Quarter] TINYINT
		,[Year] INT
		)

	CREATE TABLE #tblPARecord (
		RowNum INT
		,EmployeeId INT
		,GradeId VARCHAR(MAX)
		)

	IF @PeriodId IS NOT NULL
	BEGIN
		SELECT @FromDate = DisciplineStartDate
			,@ToDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
		FROM dbo.Period WITH (NOLOCK)
		WHERE PeriodId = @PeriodId
	END

	INSERT INTO #tblGroup (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

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
		,EmployeeId
		,CreatorId
		,GradeId
		,RoleId
		,TypeId
		,CreatedDate
		,NominationStatusId
		,NominationPeriodId
		,[Quarter]
		,[Year]
		)
	SELECT ROW_NUMBER() OVER (
			PARTITION BY record.EmployeeId ORDER BY record.EmployeeId
				,record.CreatedDate DESC
			) AS RowNum
		,record.PerformanceRecordId
		,record.EmployeeId
		,record.CreatorId
		,record.GradeId
		,record.RoleId
		,record.TypeId
		,record.CreatedDate
		,nomination.StatusId AS NominationStatusId
		,nomination.EOQYId AS NominationPeriodId
		,nominationperiod.[Quarter]
		,nominationperiod.[Year]
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		LEFT JOIN #tblGroup AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
		AND pa.StatusId = @PAClosed
		AND emp.EmployeeId = pa.EmployeeId
		LEFT JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		AND nomination.NomineeId = @EmployeeId
		LEFT JOIN dbo.EOQY AS nominationperiod WITH (NOLOCK) ON nomination.EOQYId = nominationperiod.EOQYId
		AND nominationperiod.EndDateForCommittee < @Today
	WHERE (
			(
				record.TypeId = @Checkin
				AND (
					member.IsHost = 1
					OR (
						member.IsHost = 0
						AND (
							record.CreatorId = member.EmployeeId
							OR record.CreatorId = @ActorId
							)
						)
					)
				)
			OR (
				@EmployeeId IS NOT NULL
				AND (
					(
						record.TypeId = @SystemPA
						AND pa.PARecordId IS NOT NULL
						AND period.PeriodId IS NOT NULL
						AND member.IsHost = 1
						)
					OR (
						record.TypeId = @SystemNomination
						AND nomination.NominationId IS NOT NULL
						AND nominationperiod.EOQYId IS NOT NULL
						)
					)
				)
			)
		AND (
			record.CreatorId = - 1
			OR creator.EmployeeId IS NOT NULL
			)
		AND (
			@GroupIds IS NULL
			OR gr.GroupId IS NOT NULL
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
		SELECT EmployeeId
			,COUNT(1) AS TotalItems
		FROM #tblPerformanceRecord WITH (NOLOCK)
		GROUP BY EmployeeId
		) AS record ON emp.EmployeeId = record.EmployeeId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) AS ee ON emp.EmployeeId = ee.EmployeeId
	ORDER BY ee.RankIndex

	IF @EmployeeId IS NULL
	BEGIN
		SELECT PerformanceRecordId
			,GradeId
			,RoleId
			,TypeId
			,CreatedDate
			,NominationStatusId
			,e.EmployeeId
			,e.Code
			,e.NickName
			,e.FullName
			,record.CreatorId AS EmployeeId
			,cre.Code
			,cre.NickName
			,cre.FullName
			,NominationPeriodId
			,[Quarter]
			,[Year]
		FROM #tblPerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON record.EmployeeId = e.EmployeeId
			LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
		WHERE RowNum <= @PageSize
	END
	ELSE
	BEGIN
		SELECT PerformanceRecordId
			,GradeId
			,RoleId
			,TypeId
			,CreatedDate
			,NominationStatusId
			,e.EmployeeId
			,e.Code
			,e.NickName
			,e.FullName
			,record.CreatorId AS EmployeeId
			,cre.Code
			,cre.NickName
			,cre.FullName
			,NominationPeriodId
			,[Quarter]
			,[Year]
		FROM #tblPerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON record.EmployeeId = e.EmployeeId
			LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
		ORDER BY CreatedDate DESC OFFSET(@PageIndex * @PageSize) ROWS

		FETCH NEXT @PageSize ROWS ONLY
	END

	DROP TABLE #tblGroup

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecord

	DROP TABLE #tblPARecord
END
