
CREATE PROCEDURE [dbo].[Nomination_GetByPeriodIds] @NominationPeriodIds VARCHAR(MAX)
	,@StatusId TINYINT = NULL
AS
/*
Created: 20181121@Elton.Ton
Task: get nomination.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblNominationPeriod (PeriodId INT)

	INSERT INTO #tblNominationPeriod (PeriodId)
	SELECT val
	FROM dbo.func_SplitString(@NominationPeriodIds, ',')
	WHERE ISNUMERIC(val) = 1

	SELECT n.NominationId
		,n.StatusId
		,nominee.EmployeeId
		,nominee.NickName
		,nominee.FullName
		,nominee.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,period.EOQYId AS NominationPeriodId
		,period.[Year]
		,period.[Quarter]
	FROM dbo.Nomination AS n WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
		INNER JOIN dbo.Employee AS nominee WITH (NOLOCK) ON n.NomineeId = nominee.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON nominee.GroupId = gr.GroupId
	WHERE EXISTS (
			SELECT 1
			FROM #tblNominationPeriod AS tblperiod
			WHERE period.EOQYId = tblperiod.PeriodId
			)
		AND (
			@StatusId IS NULL
			OR n.StatusId = @StatusId
			)

	SELECT pr.PerformanceRecordId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,employee.EmployeeId
		,employee.NickName
		,employee.FullName
		,employee.Code
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
		,period.EOQYId AS NominationPeriodId
		,period.[Year]
		,period.[Quarter]
	FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		INNER JOIN dbo.Employee AS employee WITH (NOLOCK) ON pr.EmployeeId = employee.EmployeeId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON pr.CreatorId = creator.EmployeeId
		INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON pr.PerformanceRecordId = nd.PerformanceRecordId
		INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON nd.NominationId = n.NominationId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
	WHERE EXISTS (
			SELECT 1
			FROM #tblNominationPeriod AS tblperiod
			WHERE period.EOQYId = tblperiod.PeriodId
			)
		AND (
			@StatusId IS NULL
			OR n.StatusId = @StatusId
			)

	DROP TABLE #tblNominationPeriod
END
