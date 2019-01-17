CREATE PROCEDURE [dbo].[NominationReport_GetNominations] 
	@NominationPeriodIds VARCHAR(MAX) = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@LoggedEmployeeId INT
	,@StatusId TINYINT = NULL
AS
/*
Created: 20181221@Elton.Ton
Task: get nomination period.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @HRSystemRoleId TINYINT = 2
		,@MNGSystemRoleId TINYINT = 3
		,@CommitteeRoleId TINYINT = 2

	SELECT val
	INTO #temp
	FROM dbo.func_SplitString(@GroupIds, ',')

	SELECT n.NominationId
		,n.EOQYId AS 'NominationPeriodId'
		,n.NomineeId
		,n.StatusId
		,n.PerformanceRecordId
	FROM dbo.Nomination n WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.Employee e WITH (NOLOCK)
			WHERE e.EmployeeId = n.NomineeId
				AND EXISTS (
					SELECT 1
					FROM #temp
					WHERE #temp.val = e.GroupId
				)
		)
		AND EXISTS(
				SELECT 1
				FROM dbo.func_SplitString(@NominationPeriodIds, ',') p
				WHERE n.EOQYId = p.val
			)
		AND (@StatusId IS NULL OR n.StatusId = @StatusId)
		AND EXISTS (SELECT 1 FROM dbo.func_SplitString(@EmployeeIds, ',') emp WHERE emp.val = n.NomineeId)
		AND (EXISTS (
				SELECT 1
				FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
				WHERE ens.EOQYId = n.EOQYId and ens.NominatorId = @LoggedEmployeeId
					AND EXISTS (SELECT 1 FROM dbo.func_SplitString(ens.RoleIds, ',') r WHERE r.val = @CommitteeRoleId)
			)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole er WITH (NOLOCK)
				WHERE er.EmployeeId = @LoggedEmployeeId
					AND (er.RoleId = @HRSystemRoleId OR er.RoleId = @MNGSystemRoleId)
			)
			OR (
				EXISTS (
					SELECT 1
					FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
					WHERE ens.EOQYId = n.EOQYId and ens.NominatorId = @LoggedEmployeeId AND ens.RoleIds = '1'
				)
				AND EXISTS (
					SELECT 1
					FROM dbo.NominationDetail nd WITH (NOLOCK)
					WHERE nd.NominationId = n.NominationId AND nd.NominatorId = @LoggedEmployeeId
				)
			)
		)

	SELECT pr.PerformanceRecordId
		,pr.Comment
		,pr.CreatedDate
		,pr.ModifiedDate
		,pr.CreatorId
		,pr.EmployeeIds
		,pr.StatusId
		,n.EOQYId AS 'NominationPeriodId'
	FROM dbo.PerformanceRecord pr WITH (NOLOCK)
		INNER JOIN dbo.NominationDetail nd WITH (NOLOCK) ON pr.PerformanceRecordId = nd.PerformanceRecordId
		INNER JOIN dbo.Nomination n WITH (NOLOCK) ON nd.NominationId = n.NominationId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.Employee e WITH (NOLOCK)
			WHERE e.EmployeeId = n.NomineeId
				AND EXISTS (
					SELECT 1
					FROM #temp
					WHERE #temp.val = e.GroupId
				)
		)
		AND EXISTS(
				SELECT 1
				FROM dbo.func_SplitString(@NominationPeriodIds, ',') p
				WHERE n.EOQYId = p.val
			)
		AND (@StatusId IS NULL OR n.StatusId = @StatusId)
		AND EXISTS (SELECT 1 FROM dbo.func_SplitString(@EmployeeIds, ',') emp WHERE emp.val = n.NomineeId)
		AND (EXISTS (
				SELECT 1
				FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
				WHERE ens.EOQYId = n.EOQYId and ens.NominatorId = @LoggedEmployeeId
					AND EXISTS (SELECT 1 FROM dbo.func_SplitString(ens.RoleIds, ',') r WHERE r.val = @CommitteeRoleId)
			)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole er WITH (NOLOCK)
				WHERE er.EmployeeId = @LoggedEmployeeId
					AND (er.RoleId = @HRSystemRoleId OR er.RoleId = @MNGSystemRoleId)
			)
			OR (
				EXISTS (
					SELECT 1
					FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
					WHERE ens.EOQYId = n.EOQYId and ens.NominatorId = @LoggedEmployeeId AND ens.RoleIds = '1'
				)
				AND nd.NominatorId = @LoggedEmployeeId
			)
		)

	DROP TABLE #temp
END
