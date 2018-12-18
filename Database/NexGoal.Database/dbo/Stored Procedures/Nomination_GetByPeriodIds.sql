CREATE PROCEDURE [dbo].[Nomination_GetByPeriodIds] 
	@NominationPeriodIds VARCHAR(MAX),
	@StatusId TINYINT = NULL
AS
/*
Created: 20181121@Elton.Ton
Task: get nomination period.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT val
	INTO #temp
	FROM dbo.func_SplitString(@NominationPeriodIds, ',')

	SELECT n.NominationId
		,n.EOQYId AS 'NominationPeriodId'
		,n.NomineeId
		,n.StatusId
		,n.PerformanceRecordId
	FROM dbo.Nomination n WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #temp t WHERE t.val = n.EOQYId
		)
		AND (@StatusId IS NULL OR n.StatusId = @StatusId)

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
		FROM #temp t WHERE t.val = n.EOQYId
	)

	DROP TABLE #temp
END
