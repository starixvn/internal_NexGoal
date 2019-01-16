
CREATE PROCEDURE [dbo].[PARecord_GetMemberPerformanceAppraisalResult] @ActorId INT
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
	DECLARE @PAClosed TINYINT = 4
		-- system role
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorRoleId TINYINT = 4
		,@ReviewerRoleId TINYINT = 5

	CREATE TABLE #tblMember (EmployeeId INT)

	INSERT INTO #tblMember (EmployeeId)
	SELECT DISTINCT EmployeeId
	FROM dbo.func_GetMember(@ActorId)
	WHERE ISNULL(@EmployeeId, EmployeeId) = EmployeeId
		AND RoleId IN (@ManagementRoleId, @EvaluatorRoleId, @ReviewerRoleId)

	SELECT pa.PARecordId
		,(CASE 
			WHEN pa.StatusId = @PAClosed THEN pa.GradeId
			ELSE NULL END
			) AS GradeId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		AND ISNULL(@PeriodId, period.PeriodId) = period.PeriodId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON pa.EmployeeId = member.EmployeeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON member.EmployeeId = emp.EmployeeId
	WHERE pa.StatusId = @PAClosed

	DROP TABLE #tblMember
END