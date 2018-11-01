
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

	SELECT data.PerformanceRecordId
		,data.GradeId
		,data.Comment
		,data.StatusId
		,data.ProjectIds
		,data.CreatedDate
		,data.ModifiedDate
		,data.RoleId
		,data.EmployeeIds
		,data.CreatorId
	FROM (
		SELECT record.PerformanceRecordId
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
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = 1
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
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
			AND creator.[Enabled] = 1
			AND creator.IsDeleted = 0
			LEFT JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		WHERE record.TypeId = 1
			AND record.StatusId IN (@Submitted, @Closed)
			AND (
				(
					-- not member
					member.EmployeeId IS NULL
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
	ORDER BY CASE 
			WHEN data.StatusId = @Draft THEN 1
			ELSE 2
			END
		,data.CreatedDate DESC

	DROP TABLE #tblMember
END
