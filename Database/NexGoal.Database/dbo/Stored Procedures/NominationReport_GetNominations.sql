CREATE PROCEDURE [dbo].[NominationReport_GetNominations] 
	@NominationPeriodIds VARCHAR(MAX) = NULL,
	@GroupIds VARCHAR(MAX) = NULL,
	@EmployeeId INT = NULL,
	@StatusId TINYINT = NULL
AS
/*
Created: 20181221@Elton.Ton
Task: get nomination period.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

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
		AND (@NominationPeriodIds IS NULL 
			OR EXISTS(
				SELECT 1
				FROM dbo.func_SplitString(@NominationPeriodIds, ',') p
				WHERE n.EOQYId = p.val
			))
		AND (@StatusId IS NULL OR n.StatusId = @StatusId)
		AND (@EmployeeId IS NULL OR n.NomineeId = @EmployeeId)

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
		AND (@NominationPeriodIds IS NULL 
			OR EXISTS(
				SELECT 1
				FROM dbo.func_SplitString(@NominationPeriodIds, ',') p
				WHERE n.EOQYId = p.val
			))
		AND (@StatusId IS NULL OR n.StatusId = @StatusId)
		AND (@EmployeeId IS NULL OR n.NomineeId = @EmployeeId)

	DROP TABLE #temp
END
