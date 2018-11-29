
CREATE PROCEDURE [dbo].[PerformanceRecord_GetByEmployee] @EmployeeId INT
AS
/*
Created: 20180912@Tim.Trinh
Task: Get PCI by employee.
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
	DECLARE @Draft TINYINT = 2
		,@Submitted TINYINT = 3
		,@Closed TINYINT = 4

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,PeriodId INT
		,GradeId VARCHAR(MAX)
		,Comment NVARCHAR(MAX)
		,StatusId TINYINT
		,ProjectIds VARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,RoleId TINYINT
		,EmployeeIds VARCHAR(MAX)
		,CreatorId INT
		,NominationStatusId TINYINT
		,NominationPeriodId INT
		,TypeId TINYINT
		)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,PeriodId
		,GradeId
		,Comment
		,StatusId
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,EmployeeIds
		,CreatorId
		,NominationStatusId
		,NominationPeriodId
		,TypeId
		)
	SELECT data.PerformanceRecordId
		,data.PeriodId
		,data.GradeId
		,data.Comment
		,data.StatusId
		,data.ProjectIds
		,data.CreatedDate
		,data.ModifiedDate
		,data.RoleId
		,data.EmployeeIds
		,data.CreatorId
		,data.NominationStatusId
		,data.NominationPeriodId
		,data.TypeId
	FROM (
		SELECT record.PerformanceRecordId
			,NULL AS PeriodId
			,record.GradeId
			,record.Comment
			,record.StatusId
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,record.EmployeeIds
			,record.CreatorId
			,record.TypeId
			,NULL AS NominationStatusId
			,NULL AS NominationPeriodId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = @Checkin
			AND record.CreatorId = @EmployeeId
			AND CAST(record.CreatorId AS VARCHAR(20)) = record.EmployeeIds
			AND record.StatusId = @Draft
		
		UNION
		SELECT record.PerformanceRecordId
			,pa.PeriodId
			,record.GradeId
			,record.Comment
			,(CASE 
				WHEN record.CreatorId != @EmployeeId THEN @Closed
				ELSE record.StatusId END
				) AS StatusId
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,record.EmployeeIds
			,record.CreatorId
			,record.TypeId
			,nomination.StatusId AS NominationStatusId
			,nomination.EOQYId AS NominationPeriodId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeIds = emp.EmployeeId
			AND emp.EmployeeId = @EmployeeId
			LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
			AND creator.[Enabled] = 1
			AND creator.IsDeleted = 0
			LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
			AND emp.EmployeeId = pa.EmployeeId
			LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
			AND record.PerformanceRecordId = nomination.PerformanceRecordId
		WHERE (
				record.TypeId = @Checkin
				OR (
					record.TypeId = @SystemPA
					AND pa.PARecordId IS NOT NULL
					)
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
		) AS data

	SELECT PerformanceRecordId
		,PeriodId
		,GradeId
		,Comment
		,StatusId
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,EmployeeIds
		,CreatorId
		,NominationStatusId
		,NominationPeriodId
		,TypeId
	FROM #tblPerformanceRecord WITH (NOLOCK)
	ORDER BY CASE 
			WHEN StatusId = @Draft THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT period.EOQYId AS NominationPeriodId
		,period.[Quarter]
		,period.[Year]
	FROM #tblPerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId

	DROP TABLE #tblPerformanceRecord
END
