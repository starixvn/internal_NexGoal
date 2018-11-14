
CREATE PROCEDURE [dbo].[PerformanceRecord_GetTimeline] @EmployeeId INT
	,@PageIndex INT = 0
	,@PageSize INT = 8
	,@TotalItems INT OUT
AS
/*
Created: 20181024@Tim.Trinh
Task: Get timeline.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5
	-- Performance Record Status
	DECLARE @Submitted TINYINT = 3
		,@Closed TINYINT = 4

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,CreatorId INT
		,GradeId VARCHAR(MAX)
		,StatusId TINYINT
		,RoleId TINYINT
		,CreatedDate DATETIME
		,NominationStatusId TINYINT
		,NominationPeriodId INT
		,TypeId TINYINT
		)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,CreatorId
		,GradeId
		,StatusId
		,RoleId
		,CreatedDate
		,NominationPeriodId
		,NominationStatusId
		,TypeId
		)
	SELECT record.PerformanceRecordId
		,record.CreatorId
		,record.GradeId
		,(CASE 
			WHEN record.CreatorId != @EmployeeId THEN @Closed
			ELSE record.StatusId END
			) AS StatusId
		,record.RoleId
		,record.CreatedDate
		,nomination.EOQYId AS NominationPeriodId
		,nomination.StatusId AS NominationStatusId
		,record.TypeId
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		AND creator.[Enabled] = 1
		AND creator.IsDeleted = 0
		LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		AND nomination.NomineeId = @EmployeeId
		AND nomination.StatusId <> 4 -- draft
	WHERE (
			record.TypeId IN (@Checkin, @SystemPA)
			OR (
				record.TypeId = @SystemNomination
				AND nomination.NominationId IS NOT NULL
				)
			)
		AND record.StatusId IN (@Submitted, @Closed)
		AND (
			record.CreatorId = - 1
			OR creator.EmployeeId IS NOT NULL
			)

	SELECT PerformanceRecordId
		,CreatorId
		,GradeId
		,StatusId
		,RoleId
		,CreatedDate
		,NominationPeriodId
		,NominationStatusId
		,TypeId
	FROM #tblPerformanceRecord WITH (NOLOCK)
	ORDER BY CreatedDate DESC 
	OFFSET(@PageIndex * @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY
	
	SELECT period.EOQYId AS NominationPeriodId
		,period.[Quarter]
		,period.[Year]
	FROM #tblPerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId

	SELECT e.EOQYId AS 'NominationPeriodId'
		,e.[Quarter]
		,e.[Year]
	FROM dbo.EOQY e WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #tblPerformanceRecord tpr WITH (NOLOCK)
			WHERE tpr.NominationPeriodId = e.EOQYId
		)

	SELECT @TotalItems = COUNT(1)
	FROM #tblPerformanceRecord

	DROP TABLE #tblPerformanceRecord
END