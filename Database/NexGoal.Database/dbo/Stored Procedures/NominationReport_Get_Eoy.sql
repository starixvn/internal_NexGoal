CREATE PROCEDURE [dbo].[NominationReport_Get_Eoy] @ActorId INT
	,@NominationPeriodIds VARCHAR(MAX) = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20221207@Otis.Vu
Task: get nomination report.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Is_HR_MNG BIT = 0
		,@HRSystemRoleId TINYINT = 2
		,@MNGSystemRoleId TINYINT = 3
		,@NominatorRoleId TINYINT = 1
		,@CommitteeRoleId TINYINT = 2

	CREATE TABLE #tblNominationPeriod (PeriodId INT)

	CREATE TABLE #tblGroup (GroupId INT)

	CREATE TABLE #tblEmployee (EmployeeId INT)

	INSERT INTO #tblNominationPeriod (PeriodId)
	SELECT val
	FROM dbo.func_SplitString(@NominationPeriodIds, ',')

	INSERT INTO #tblGroup (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')

	INSERT INTO #tblEmployee (EmployeeId)
	SELECT val
	FROM dbo.func_SplitString(@EmployeeIds, ',')

	SELECT @Is_HR_MNG = CASE 
		WHEN EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole WITH (NOLOCK)
				WHERE EmployeeId = @ActorId
					AND (
						RoleId = @HRSystemRoleId
						OR RoleId = @MNGSystemRoleId
						)
				) THEN 1
		ELSE 0
		END

	SELECT n.NominationId
		,n.StatusId
		,nominee.EmployeeId
		,nominee.NickName
		,nominee.FullName
		,nominee.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,n.EOQYId AS NominationPeriodId
		,period.[Year]
		,period.[Quarter]
	FROM dbo.Nomination AS n WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON n.EOQYId = period.EOQYId
		INNER JOIN dbo.Employee AS nominee WITH (NOLOCK) ON n.NomineeId = nominee.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON nominee.GroupId = gr.GroupId
	WHERE EXISTS (
			SELECT 1
			FROM #tblNominationPeriod AS tblperiod
			WHERE n.EOQYId = tblperiod.PeriodId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroup AS tblgr
			WHERE nominee.GroupId = tblgr.GroupId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblEmployee AS tblemp
			WHERE nominee.EmployeeId = tblemp.EmployeeId
			)
		AND (
			@StatusId IS NULL
			OR n.StatusId = @StatusId
			)
		AND (
			@Is_HR_MNG = 1
			OR EXISTS (
				SELECT 1
				FROM dbo.EOQYNominatorSetting AS ens WITH (NOLOCK)
					CROSS APPLY dbo.func_SplitString(ens.RoleIds, ',') AS cr
				WHERE ens.EOQYId = n.EOQYId
					AND ens.NominatorId = @ActorId
					AND (
						cr.val = @CommitteeRoleId
						OR (
							cr.val = @NominatorRoleId
							AND EXISTS (
								SELECT 1
								FROM dbo.NominationDetail AS nd WITH (NOLOCK)
								WHERE nd.NominationId = n.NominationId
									AND nd.NominatorId = @ActorId
								)
							)
						)
				)
			)
	
	SELECT pr.PerformanceRecordId
		,nkg.KPIGroupId
		,nkg.Rate
		,nk.KPIId
		,nk.Comment
		,kpig.Name
		,kpi.Name AS KPIName
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
		INNER JOIN dbo.NominationKPIGroup AS nkg ON nd.NominationId = nkg.NominationId 
		AND nd.NominatorId = nkg.NominatorId
		INNER JOIn dbo.NominationKPI AS nk ON nkg.KPIGroupId = nk.KPIGroupId 
		AND nkg.NominatorId = nk.NominatorId AND nkg.NominationId = nk.NominationId
		INNER JOIN dbo.KPI AS kpi ON kpi.KPIId = nk.KPIId AND kpi.KPIGroupId = nk.KPIGroupId
		INNER JOIN dbo.KPIGroup AS kpig ON kpig.KPIGroupId = kpi.KPIGroupId
	WHERE EXISTS (
			SELECT 1
			FROM #tblNominationPeriod AS tblperiod
			WHERE n.EOQYId = tblperiod.PeriodId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroup AS tblgr
			WHERE employee.GroupId = tblgr.GroupId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblEmployee AS tblemp
			WHERE employee.EmployeeId = tblemp.EmployeeId
			)
		AND (
			@StatusId IS NULL
			OR n.StatusId = @StatusId
			)
		AND (
			@Is_HR_MNG = 1
			OR EXISTS (
				SELECT 1
				FROM dbo.EOQYNominatorSetting AS ens WITH (NOLOCK)
					CROSS APPLY dbo.func_SplitString(ens.RoleIds, ',') AS cr
				WHERE ens.EOQYId = n.EOQYId
					AND ens.NominatorId = @ActorId
					AND (
						cr.val = @CommitteeRoleId
						OR (
							cr.val = @NominatorRoleId
							AND nd.NominatorId = @ActorId
							)
						)
				)
			)

	DROP TABLE #tblNominationPeriod
	
	DROP TABLE #tblGroup
	
	DROP TABLE #tblEmployee
END
