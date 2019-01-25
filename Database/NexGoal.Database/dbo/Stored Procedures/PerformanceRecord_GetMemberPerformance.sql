


CREATE PROCEDURE [dbo].[PerformanceRecord_GetMemberPerformance] @ActorId INT
AS
/*
Created: 20181026@Tim.Trinh
Task: Get member PCI.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@Checkin TINYINT = 1


	CREATE TABLE #tblMember (
		EmployeeId INT
		,IsHost BIT
		)

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,GradeId VARCHAR(MAX)
		,Comment NVARCHAR(MAX)
		,IsDraft BIT
		,ProjectIds VARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,RoleId TINYINT
		,EmployeeIds VARCHAR(MAX)
		,CreatorId INT
		,TypeId TINYINT
		)

	INSERT INTO #tblMember (
		EmployeeId
		,IsHost
		)
	SELECT data.EmployeeId
		,MAX(data.IsHost)
	FROM (
		SELECT EmployeeId
			,(CASE 
				WHEN ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId) THEN 1
				ELSE 0 END
				) AS IsHost
		FROM dbo.func_GetMember(@ActorId)
		WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		) AS data
	GROUP BY data.EmployeeId

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,EmployeeIds
		,CreatorId
		,TypeId
		)
	SELECT record.DraftPerformanceRecordId AS PerformanceRecordId
		,record.GradeId
		,record.Comment
		,1 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.EmployeeIds
		,record.CreatorId
		,record.TypeId
	FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
	WHERE record.EmployeeIds <> CAST(record.CreatorId AS VARCHAR(20))
		AND record.CreatorId = @ActorId
		
	UNION
	SELECT record.PerformanceRecordId
			,record.GradeId
			,record.Comment
			,0 AS IsDraft
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,CAST(record.EmployeeId AS VARCHAR(20))
			,record.CreatorId
			,record.TypeId
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
		
	WHERE record.TypeId = @Checkin
		AND (
			member.IsHost = 1
			OR (
				member.IsHost = 0
				AND (
					record.CreatorId = member.EmployeeId
					OR record.CreatorId = @ActorId
					)
				)
			)

	SELECT
		PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,record.CreatorId AS EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
	FROM #tblPerformanceRecord AS record
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	ORDER BY CASE 
			WHEN IsDraft = 1 THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT record.PerformanceRecordId AS LinkObjectId
		,e.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.EmployeeIds, ',') a
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON a.val = e.EmployeeId

	SELECT record.PerformanceRecordId
		,prj.ProjectId
		,prj.[Name] 
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.Project AS prj WITH (NOLOCK) ON a.val = prj.ProjectId

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecord
END
