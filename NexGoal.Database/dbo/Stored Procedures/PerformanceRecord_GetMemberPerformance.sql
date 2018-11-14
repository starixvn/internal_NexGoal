
CREATE PROCEDURE [dbo].[PerformanceRecord_GetMemberPerformance] @ActorId INT
	,@EmployeeId INT = NULL
AS
/*
Created: 20181026@Tim.Trinh
Task: Get member PCI.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeIdString VARCHAR(20)
	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5
	-- Performance Record Status
	DECLARE @Draft TINYINT = 2
		,@Submitted TINYINT = 3
		,@Closed TINYINT = 4

	SET @EmployeeIdString = CASE 
			WHEN @EmployeeId IS NULL THEN ''
			ELSE CAST(@EmployeeId AS VARCHAR(20))
			END

	CREATE TABLE #tblMember (
		EmployeeId INT
		,IsHost BIT
		)

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,PeriodId INT
		,GradeId VARCHAR(MAX)
		,Comment NVARCHAR(MAX)
		,StatusId TINYINT
		,ProjectIds VARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,RoleId TINYINT
		,EmployeeIds VARCHAR(MAX)
		,CreatorId INT
		,NominationStatusId TINYINT
		,NominationPeriodId INT
		,TypeId TINYINT
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
		PerformanceRecordId
		,PeriodId
		,GradeId
		,Comment
		,StatusId
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,EmployeeIds
		,CreatorId
		,NominationStatusId
		,NominationPeriodId
		,TypeId
		)
	SELECT data.PerformanceRecordId
		,data.PeriodId
		,data.GradeId
		,data.Comment
		,data.StatusId
		,data.ProjectIds
		,data.CreatedDate
		,data.ModifiedDate
		,data.RoleId
		,data.EmployeeIds
		,data.CreatorId
		,data.NominationStatusId
		,data.NominationPeriodId
		,data.TypeId
	FROM (
		SELECT record.PerformanceRecordId
			,NULL AS PeriodId
			,record.GradeId
			,record.Comment
			,(CASE 
				WHEN record.CreatorId != @ActorId THEN @Closed
				ELSE record.StatusId END
				) AS StatusId
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,record.EmployeeIds
			,record.CreatorId
			,record.TypeId
			,NULL AS NominationStatusId
			,NULL AS NominationPeriodId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = @Checkin
			AND record.EmployeeIds <> CAST(record.CreatorId AS VARCHAR(20))
			AND record.CreatorId = @ActorId
			AND record.StatusId = @Draft
			AND (
				@EmployeeIdString = ''
				OR record.EmployeeIds LIKE @EmployeeIdString
				OR record.EmployeeIds LIKE @EmployeeIdString + ',%'
				OR record.EmployeeIds LIKE '%,' + @EmployeeIdString + ',%'
				OR record.EmployeeIds LIKE '%,' + @EmployeeIdString
				)
		
		UNION
		SELECT record.PerformanceRecordId
			,pa.PeriodId
			,record.GradeId
			,record.Comment
			,(CASE 
				WHEN record.CreatorId != @ActorId THEN @Closed
				ELSE record.StatusId END
				) AS StatusId
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,record.EmployeeIds
			,record.CreatorId
			,record.TypeId
			,nomination.StatusId AS NominationStatusId
			,nomination.EOQYId AS NominationPeriodId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
			AND creator.[Enabled] = 1
			AND creator.IsDeleted = 0
			LEFT JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
			LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
			AND emp.EmployeeId = pa.EmployeeId
			LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
			AND record.PerformanceRecordId = nomination.PerformanceRecordId
			AND nomination.StatusId <> 4 -- draft
		WHERE (
				record.TypeId IN (@Checkin, @SystemNomination)
				OR (
					record.TypeId = @SystemPA
					AND @EmployeeId IS NOT NULL
					)
				)
			AND record.StatusId IN (@Submitted, @Closed)
			AND (
				record.CreatorId = - 1
				OR creator.EmployeeId IS NOT NULL
				)
			AND (
				(
					-- not member
					@EmployeeIdString = ''
					AND member.EmployeeId IS NULL
					AND record.CreatorId = @ActorId
					AND record.EmployeeIds <> @ActorId
					)
				OR (
					member.EmployeeId IS NOT NULL
					AND (
						member.IsHost = 1
						OR (
							member.IsHost = 0
							AND record.CreatorId = @ActorId
							)
						)
					)
				)
		) AS data

	SELECT PerformanceRecordId
		,PeriodId
		,GradeId
		,Comment
		,StatusId
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,EmployeeIds
		,CreatorId
		,NominationStatusId
		,NominationPeriodId
		,TypeId
	FROM #tblPerformanceRecord WITH (NOLOCK)
	ORDER BY CASE 
			WHEN StatusId = @Draft THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT period.EOQYId AS NominationPeriodId
		,period.[Quarter]
		,period.[Year]
	FROM #tblPerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecord
END
