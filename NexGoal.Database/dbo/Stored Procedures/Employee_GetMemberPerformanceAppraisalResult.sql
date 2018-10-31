
CREATE PROCEDURE [dbo].[Employee_GetMemberPerformanceAppraisalResult] @ActorId INT
	,@PeriodId INT = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20181004@Tim.Trinh
Task: Get member PA result.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		-- system role
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorRoleId TINYINT = 4
		,@ReviewerRoleId TINYINT = 5
		,@PMRoleId TINYINT = 6

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
				WHEN RoleId IN (@ManagementRoleId, @ReviewerRoleId) THEN 1
				ELSE 0 END
				) AS IsHost
		FROM dbo.func_GetMember(@ActorId)
		WHERE ISNULL(@EmployeeId, EmployeeId) = EmployeeId
		) AS data
	GROUP BY data.EmployeeId

	SELECT pa.PARecordId
		,pa.EmployeeId
		,pa.PeriodId
		,(CASE 
			WHEN pa.StatusId = @PAClosed
				OR member.IsHost = 1 THEN pa.GradeId
			ELSE NULL END
			) AS GradeId
		,(CASE 
			WHEN pa.StatusId = @PAClosed
				OR member.IsHost = 1 THEN pa.Comment
			ELSE NULL END
			) AS Comment
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		AND ISNULL(@PeriodId, period.PeriodId) = period.PeriodId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON pa.EmployeeId = member.EmployeeId
	WHERE pa.StatusId IN (@PAInProgress, @PAClosed)

	SELECT DISTINCT period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		AND ISNULL(@PeriodId, period.PeriodId) = period.PeriodId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON pa.EmployeeId = member.EmployeeId
	WHERE pa.StatusId IN (@PAInProgress, @PAClosed)

	DROP TABLE #tblMember
END
