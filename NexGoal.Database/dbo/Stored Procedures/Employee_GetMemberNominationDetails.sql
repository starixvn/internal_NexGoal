
CREATE PROCEDURE [dbo].[Employee_GetMemberNominationDetails] @ActorId INT
	,@NominationPeriodId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20181004@Tim.Trinh
Task: Get member nomination details.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@NominationRoleId TINYINT
		,@NominatorRoleId TINYINT = 1
		,@CommitteeRoleId TINYINT = 2
		-- system role
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorRoleId TINYINT = 4
		,@ReviewerRoleId TINYINT = 5
		,@PMRoleId TINYINT = 6

	CREATE TABLE #tblMember (
		EmployeeId INT
		,IsHost BIT
		)

	CREATE TABLE #tblNominationPeriod (
		NominationPeriodId INT
		,[Quarter] TINYINT NULL
		,[Year] INT
		,IsClosed BIT
		)

	SELECT TOP 1 @NominationRoleId = (
			SELECT MAX(val)
			FROM dbo.func_SplitString(setting.RoleIds, ',')
			)
	FROM dbo.EOQY AS period WITH (NOLOCK)
		INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON period.EOQYId = setting.EOQYId
		AND setting.NominatorId = @ActorId
	WHERE period.StartDateForNominator <= @Today
	ORDER BY period.StartDateForNominator DESC

	INSERT INTO #tblMember (
		EmployeeId
		,IsHost
		)
	SELECT data.EmployeeId
		,MAX(data.IsHost)
	FROM (
		SELECT EmployeeId
			,(CASE 
				WHEN RoleId IN (@ManagementRoleId, @EvaluatorRoleId, @ReviewerRoleId) THEN 1
				ELSE 0 END
				) AS IsHost
		FROM dbo.func_GetMember(@ActorId)
		WHERE ISNULL(@EmployeeId, EmployeeId) = EmployeeId
		) AS data
	GROUP BY data.EmployeeId

	INSERT INTO #tblNominationPeriod (
		NominationPeriodId
		,[Quarter]
		,[Year]
		,IsClosed
		)
	SELECT eoqy.EOQYId
		,eoqy.[Quarter]
		,eoqy.[Year]
		,CASE 
			WHEN eoqy.EndDateForCommittee < @Today THEN 1
			ELSE 0
			END
	FROM dbo.EOQY AS eoqy WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON eoqy.EOQYId = nomination.EOQYId
	WHERE ISNULL(eoqy.EOQYId, @NominationPeriodId) = eoqy.EOQYId
		AND eoqy.StartDateForNominator < @Today

	-- result
	SELECT NominationPeriodId
		,[Quarter]
		,[Year]
	FROM #tblNominationPeriod WITH (NOLOCK)

	SELECT emp.EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,period.NominationPeriodId
		,nomination.StatusId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
		INNER JOIN #tblNominationPeriod AS period WITH (NOLOCK) ON nomination.EOQYId = period.NominationPeriodId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId

	SELECT period.NominationPeriodId
		,detail.NominatorId
		,record.Comment
		,record.CreatedDate
		,record.ModifiedDate
	FROM dbo.Nomination AS nomination WITH (NOLOCK)
		INNER JOIN #tblNominationPeriod AS period WITH (NOLOCK) ON nomination.EOQYId = period.NominationPeriodId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON nomination.NomineeId = member.EmployeeId
		INNER JOIN dbo.NominationDetail AS detail WITH (NOLOCK) ON nomination.NominationId = detail.NominationId
		INNER JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON detail.PerformanceRecordId = record.PerformanceRecordId
	WHERE (
			period.IsClosed = 1
			AND (
				member.IsHost = 1
				OR @NominationRoleId IN (@NominatorRoleId, @CommitteeRoleId)
				OR detail.NominatorId = @ActorId
				)
			)
		OR (
			period.IsClosed = 0
			AND (
				@NominationRoleId = @CommitteeRoleId
				OR (
					@NominationRoleId = @NominatorRoleId
					AND detail.NominatorId = @ActorId
					)
				)
			)

	DROP TABLE #tblMember

	DROP TABLE #tblNominationPeriod
END
