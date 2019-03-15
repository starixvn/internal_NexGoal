
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

	DECLARE @Today DATE = GETDATE()
		,@PAClosed TINYINT = 4
	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,GradeId VARCHAR(MAX)
		,Comment NVARCHAR(MAX)
		,IsDraft BIT
		,ProjectIds VARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,RoleId TINYINT
		,TypeId TINYINT
		,EmployeeId INT
		,CreatorId INT
		,NominationPeriodId INT
		,NominationStatusId TINYINT
		,[Quarter] TINYINT
		,[Year] INT
		,PeriodId INT
		,PeriodName NVARCHAR(300)
		)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,EmployeeId
		,CreatorId
		,NominationPeriodId
		,NominationStatusId
		,[Quarter]
		,[Year]
		,PeriodId
		,PeriodName
		)
	-- draft performance record
	SELECT record.DraftPerformanceRecordId AS PerformanceRecordId
		,record.GradeId
		,record.Comment
		,1 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,emp.EmployeeId
		,record.CreatorId
		,NULL AS NominationPeriodId
		,NULL AS NominationStatusId
		,NULL AS [Quarter]
		,NULL AS [Year]
		,NULL AS PeriodId
		,NULL AS PeriodName
	FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON (CASE 
			WHEN record.EmployeeIds NOT LIKE '%[^0-9]%' THEN CAST(record.EmployeeIds AS INT)
			ELSE - 1 END
			) = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.CreatorId = @EmployeeId
	
	UNION
	-- performance record
	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,record.EmployeeId
		,record.CreatorId
		,NULL AS NominationPeriodId
		,NULL AS NominationStatusId
		,NULL AS [Quarter]
		,NULL AS [Year]
		,NULL AS PeriodId
		,NULL AS PeriodName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.TypeId = @Checkin
	
	UNION
	-- system PA
	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,record.EmployeeId
		,record.CreatorId
		,NULL AS NominationPeriodId
		,NULL AS NominationStatusId
		,NULL AS [Quarter]
		,NULL AS [Year]
		,p.PeriodId AS PeriodId
		,p.[Name] AS PeriodName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
		AND pa.StatusId = @PAClosed
		AND emp.EmployeeId = pa.EmployeeId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON pa.PeriodId = p.PeriodId
		AND p.IsActive = 1
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.TypeId = @SystemPA
	
	UNION
	-- nomination result
	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,record.EmployeeId
		,record.CreatorId
		,nomination.EOQYId AS NominationPeriodId
		,nomination.StatusId AS NominationStatusId
		,period.[Quarter]
		,period.[Year]
		,NULL AS PeriodId
		,NULL AS PeriodName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		LEFT JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EndDateForCommittee IS NOT NULL
		AND period.EndDateForCommittee < @Today
	WHERE record.TypeId = @SystemNomination
		AND nomination.NominationId IS NOT NULL
		AND period.EOQYId IS NOT NULL

	SELECT PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,NominationStatusId
		,e.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,record.CreatorId AS EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
		,NominationPeriodId
		,[Quarter]
		,[Year]
		,PeriodId
		,PeriodName AS [Name]
	FROM #tblPerformanceRecord AS record
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON record.EmployeeId = e.EmployeeId
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	ORDER BY CASE 
			WHEN IsDraft = 1 THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT record.PerformanceRecordId
		,prj.ProjectId
		,prj.[Name]
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.ProjectTag AS prj WITH (NOLOCK) ON a.val = prj.ProjectId

	DROP TABLE #tblPerformanceRecord
END
