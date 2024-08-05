CREATE PROCEDURE [dbo].[PRTracking_Get] @ActorId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@NoPR TINYINT = NULL
AS
/*
Created: 20190506@Elton.Ton
Task: Get PA Report.
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 5
		,@EvaluatorProcessId TINYINT = 4
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@PSTRoleId TINYINT = 7
		,@Today DATE = GETDATE()
		,@WorkingFromDate DATE
		,@WorkingToDate DATE
		,@PeriodId INT

	SELECT @PeriodId = p.PeriodId
		,@WorkingFromDate = p.DisciplineStartDate
		,@WorkingToDate = p.DisciplineEndDate
	FROM dbo.Period p WITH (NOLOCK)
	WHERE @Today BETWEEN p.DisciplineStartDate
			AND p.DisciplineEndDate
		AND p.IsActive = 1

	IF @PeriodId IS NULL RETURN

	CREATE TABLE #tmpMember (EmployeeId INT)

	SELECT val
	INTO #tmpSelectedEmployee
	FROM dbo.func_SplitString(@EmployeeIds, ',')

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole AS er WITH (NOLOCK)
			WHERE er.EmployeeId = @ActorId
				AND er.RoleId IN (@HRRoleId, @ManagementRoleId, @PSTRoleId)
			)
	BEGIN
		INSERT INTO #tmpMember (EmployeeId)
		SELECT e.EmployeeId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE e.IsDeleted = 0
			AND e.[Enabled] = 1
			AND EXISTS (
				SELECT 1
				FROM #tmpSelectedEmployee AS se WITH (NOLOCK)
				WHERE se.val = EmployeeId
				)
	END
	ELSE
	BEGIN
		INSERT INTO #tmpMember (EmployeeId)
		SELECT val
		FROM #tmpSelectedEmployee AS se WITH (NOLOCK)
		WHERE EXISTS (
				SELECT 1
				FROM dbo.func_GetMember(@ActorId)
				WHERE se.val = EmployeeId
				)
			OR se.val = @ActorId
	END

	SELECT e.EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
		,jt.[Name] AS JobTitle
		,g.[Name] AS GroupName
	INTO #tmpResult
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE EXISTS (
			SELECT 1
			FROM #tmpMember AS m WITH (NOLOCK)
			WHERE m.EmployeeId = e.EmployeeId
			)

	SELECT e.EmployeeId
		,e.EmployeeId AS CreatorId
		,e.NickName AS CreatorNickname
		,e.FullName AS CreatorFullname
		,@SelfProcessId AS RoleId
		,selfPR.Quantity
	INTO #tmpDetail
	FROM #tmpMember AS m WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON m.EmployeeId = e.EmployeeId
		CROSS APPLY (
		SELECT COUNT(1) AS Quantity
		FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		WHERE m.EmployeeId = pr.EmployeeId
			AND pr.TypeId = 1
			AND pr.RoleId = @SelfProcessId
			AND pr.CreatorId = pr.EmployeeId
			AND pr.CreatedDate BETWEEN @WorkingFromDate
				AND @WorkingToDate
		) selfPR

	INSERT INTO #tmpDetail
	SELECT efs.EmployeeId
		,e.EmployeeId AS CreatorId
		,e.NickName AS CreatorNickname
		,e.FullName AS CreatorFullname
		,efs.ProcessId AS RoleId
		,COUNT(pr.PerformanceRecordId) AS Quantity
	FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON efs.EvaluatorId = e.EmployeeId
		AND efs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
		LEFT JOIN PerformanceRecord AS pr WITH (NOLOCK) ON efs.EvaluatorId = pr.CreatorId
		AND pr.EmployeeId = efs.EmployeeId
		AND pr.TypeId = 1
		AND pr.RoleId = efs.ProcessId
		AND pr.CreatedDate BETWEEN @WorkingFromDate
			AND @WorkingToDate
	WHERE EXISTS (
			SELECT 1
			FROM #tmpMember AS m WITH (NOLOCK)
			WHERE m.EmployeeId = efs.EmployeeId
			)
	GROUP BY efs.EmployeeId
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,efs.ProcessId

	INSERT INTO #tmpDetail
	SELECT member.EmployeeId
		,e.EmployeeId AS CreatorId
		,e.NickName AS CreatorNickname
		,e.FullName AS CreatorFullname
		,gfs.ProcessId AS RoleId
		,COUNT(pr.PerformanceRecordId) AS Quantity
	FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS member WITH (NOLOCK) ON gfs.GroupId = member.GroupId
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON gfs.EvaluatorId = e.EmployeeId
		AND gfs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
		LEFT JOIN PerformanceRecord pr WITH (NOLOCK) ON gfs.EvaluatorId = pr.CreatorId
		AND pr.EmployeeId = member.EmployeeId
		AND pr.TypeId = 1
		AND pr.RoleId = gfs.ProcessId
		AND pr.CreatedDate BETWEEN @WorkingFromDate
			AND @WorkingToDate
	WHERE EXISTS (
			SELECT 1
			FROM #tmpMember AS m WITH (NOLOCK)
			WHERE m.EmployeeId = member.EmployeeId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM EmployeeFlowSetting AS efs WITH (NOLOCK)
			WHERE member.EmployeeId = efs.EmployeeId
			)
	GROUP BY member.EmployeeId
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,gfs.ProcessId

	INSERT INTO #tmpDetail
	SELECT pms.MemberId AS EmployeeId
		,e.EmployeeId AS CreatorId
		,e.NickName AS CreatorNickname
		,e.FullName AS CreatorFullname
		,@PMProcessId AS RoleId
		,PMPR.Quantity
	FROM dbo.PMSetting AS pms WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pms.ProjectManagerId = e.EmployeeId
		CROSS APPLY (
		SELECT COUNT(1) AS Quantity
		FROM PerformanceRecord AS pr WITH (NOLOCK)
		WHERE pr.EmployeeId = pms.MemberId
			AND pr.CreatorId = pms.ProjectManagerId
			AND pr.TypeId = 1
			AND pr.RoleId = @PMProcessId
			AND pr.CreatedDate BETWEEN @WorkingFromDate
				AND @WorkingToDate
		) PMPR
	WHERE EXISTS (
			SELECT 1
			FROM #tmpMember AS m WITH (NOLOCK)
			WHERE m.EmployeeId = pms.MemberId
			)

	SELECT p.PeriodId
		,p.[Name]
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

	SELECT r.EmployeeId
		,NickName
		,FullName
		,GroupId
		,JobTitle
		,GroupName
	FROM #tmpResult AS r WITH (NOLOCK)
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) se ON r.EmployeeId = se.EmployeeId
	WHERE @NoPR IS NULL
		OR (
			@NoPR IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId, @SelfProcessId)
			AND NOT EXISTS (
				SELECT 1
				FROM #tmpDetail AS d WITH (NOLOCK)
				WHERE d.EmployeeId = r.EmployeeId
					AND d.RoleId = @NoPR
				HAVING MIN(d.Quantity) > 0
				)
			)
	ORDER BY se.RankIndex

	SELECT d.EmployeeId
		,d.CreatorId
		,d.CreatorNickname
		,d.CreatorFullname
		,d.RoleId
		,d.Quantity
	FROM #tmpDetail AS d WITH (NOLOCK)
	WHERE @NoPR IS NULL
		OR (
			@NoPR IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId, @SelfProcessId)
			AND NOT EXISTS (
				SELECT 1
				FROM #tmpDetail AS d1 WITH (NOLOCK)
				WHERE d1.EmployeeId = d.EmployeeId
					AND d1.RoleId = @NoPR
				HAVING MIN(d1.Quantity) > 0
				)
			)

	DROP TABLE #tmpMember

	DROP TABLE #tmpSelectedEmployee

	DROP TABLE #tmpResult

	DROP TABLE #tmpDetail
END

