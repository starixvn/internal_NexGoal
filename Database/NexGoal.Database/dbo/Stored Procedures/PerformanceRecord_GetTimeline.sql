
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

	DECLARE @Today DATE = GETDATE()
		,@PAClosed TINYINT = 4
	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,CreatorId INT
		,TypeId TINYINT
		,GradeId VARCHAR(MAX)
		,RoleId TINYINT
		,CreatedDate DATETIME
		,NominationStatusId TINYINT
		,NominationPeriodId INT
		,[Quarter] TINYINT
		,[Year] INT
		)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,CreatorId
		,GradeId
		,RoleId
		,CreatedDate
		,NominationPeriodId
		,[Quarter]
		,[Year]
		,NominationStatusId
		,TypeId
		)
	SELECT record.PerformanceRecordId
		,record.CreatorId
		,record.GradeId
		,record.RoleId
		,record.CreatedDate
		,nomination.EOQYId AS NominationPeriodId
		,period.[Quarter]
		,period.[Year]
		,nomination.StatusId AS NominationStatusId
		,record.TypeId
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
		AND pa.StatusId = @PAClosed
		AND record.EmployeeId = pa.EmployeeId
		LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		AND nomination.NomineeId = @EmployeeId
		LEFT JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EndDateForCommittee < @Today
	WHERE record.EmployeeId = @EmployeeId
		AND (
			record.TypeId = @Checkin
			OR (
				record.TypeId = @SystemPA
				AND pa.PARecordId IS NOT NULL
				)
			OR (
				record.TypeId = @SystemNomination
				AND period.EOQYId IS NOT NULL
				)
			)

	SELECT PerformanceRecordId
		,GradeId
		,RoleId
		,TypeId
		,CreatedDate
		,NominationStatusId
		,record.CreatorId AS EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
		,NominationPeriodId
		,[Quarter]
		,[Year]
	FROM #tblPerformanceRecord AS record WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	ORDER BY CreatedDate DESC OFFSET(@PageIndex * @PageSize) ROWS

	FETCH NEXT @PageSize ROWS ONLY

	SELECT @TotalItems = COUNT(1)
	FROM #tblPerformanceRecord

	DROP TABLE #tblPerformanceRecord
END