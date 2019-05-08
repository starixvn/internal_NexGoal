CREATE PROCEDURE [dbo].[PRTracking_Get] 
	@ActorId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@NoPR BIT = 0
AS
/*
Created: 20190506@Elton.Ton
Task: Get PR Report.
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
	WHERE @Today BETWEEN p.DisciplineStartDate AND p.StartDate AND p.IsActive = 1

	CREATE TABLE #member(EmployeeId INT)

	SELECT val
	INTO #selectedEmp
	FROM dbo.func_SplitString(@EmployeeIds,',')

	IF EXISTS (
		SELECT 1
		FROM dbo.EmployeeRole AS er WITH (NOLOCK)
		WHERE er.EmployeeId = @ActorId 
			AND er.RoleId IN (@HRRoleId, @ManagementRoleId, @PSTRoleId)
	)
	BEGIN
		INSERT INTO #member(EmployeeId)
		SELECT e.EmployeeId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE e.IsDeleted = 0
			AND e.[Enabled] = 1
			AND EXISTS (
				SELECT 1
				FROM #selectedEmp se
				WHERE se.val = EmployeeId
			)
	END
	ELSE
	BEGIN
		INSERT INTO #member(EmployeeId)
		SELECT val
		FROM #selectedEmp se
		WHERE EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@ActorId)
			WHERE se.val = EmployeeId
		) OR se.val = @ActorId
	END

	SELECT e.EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
		,jt.Name AS JobTitle
		,g.Name AS GroupName
		,(
			SELECT COUNT(*)
			FROM dbo.PerformanceRecord pr WITH (NOLOCK) 				
			WHERE pr.CreatorId = e.EmployeeId AND pr.EmployeeId = pr.CreatorId
				AND pr.TypeId = 1 AND pr.CreatedDate BETWEEN @WorkingFromDate AND @WorkingToDate
		) AS SelfPRs
		,(
			SELECT COUNT(*)
			FROM dbo.PMSetting pms WITH (NOLOCK)
				INNER JOIN dbo.PerformanceRecord pr WITH (NOLOCK) 
				ON pms.MemberId = pr.EmployeeId AND pms.ProjectManagerId = pr.CreatorId AND pr.RoleId = @PMProcessId
					AND pr.TypeId = 1 AND pr.CreatedDate BETWEEN @WorkingFromDate AND @WorkingToDate
			WHERE pms.MemberId = e.EmployeeId
		) AS PMPRs
		,(
			SELECT COUNT(*) AS 'Count'
			FROM dbo.PerformanceRecord pr WITH (NOLOCK) 
			WHERE e.EmployeeId = pr.EmployeeId AND pr.TypeId = 1 
				AND pr.CreatedDate BETWEEN @WorkingFromDate AND @WorkingToDate
				AND pr.RoleId = @EvaluatorProcessId
		) AS EvaluatorPRs
		,(
			SELECT COUNT(*) AS 'Count'
			FROM dbo.PerformanceRecord pr WITH (NOLOCK) 
			WHERE e.EmployeeId = pr.EmployeeId AND pr.TypeId = 1 
				AND pr.CreatedDate BETWEEN @WorkingFromDate AND @WorkingToDate
				AND pr.RoleId = @ReviewerProcessId
		) AS ReviewerPRs
	INTO #result
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN dbo.JobTitle jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE EXISTS (SELECT 1 from #member m WHERE m.EmployeeId = e.EmployeeId)

	SELECT p.PeriodId
		,p.Name
	FROM dbo.Period p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

	SELECT EmployeeId
		,NickName
		,FullName
		,GroupId
		,JobTitle
		,GroupName	
		,SelfPRs
		,PMPRs
		,EvaluatorPRs
		,ReviewerPRs
	FROM #result		
	WHERE @NoPR = 0 OR SelfPRs = 0 OR PMPRs = 0 OR EvaluatorPRs = 0 OR ReviewerPRs = 0
	ORDER BY GroupName, NickName

	DROP TABLE #member
	DROP TABLE #selectedEmp
	DROP TABLE #result
END
