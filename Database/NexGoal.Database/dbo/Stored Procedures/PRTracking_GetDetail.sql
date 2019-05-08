CREATE PROCEDURE [dbo].[PRTracking_GetDetail] 
	@EmployeeId INT
	,@ProcessId INT
AS
/*
Created: 20190506@Elton.Ton
Task: Get PR tracking detail.
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PMProcessId TINYINT = 3
		,@Today DATE = GETDATE()
		,@WorkingFromDate DATE
		,@WorkingToDate DATE

	SELECT @WorkingFromDate = p.DisciplineStartDate
		,@WorkingToDate = p.DisciplineEndDate
	FROM dbo.Period p WITH (NOLOCK)
	WHERE @Today BETWEEN p.DisciplineStartDate AND p.StartDate AND p.IsActive = 1

	IF @ProcessId = @PMProcessId
	BEGIN
		SELECT e.EmployeeId
			,e.NickName
			,e.FullName
			,PMPR.Quantity
		FROM dbo.PMSetting pms WITH (NOLOCK)
			INNER JOIN dbo.Employee e WITH (NOLOCK) ON pms.ProjectManagerId = e.EmployeeId
			CROSS APPLY	(
				SELECT COUNT(*) AS Quantity
				FROM PerformanceRecord pr WITH (NOLOCK)
				WHERE pr.EmployeeId = @EmployeeId AND pr.CreatorId = pms.ProjectManagerId
					AND pr.TypeId = 1 AND pr.RoleId = @ProcessId
					AND pr.CreatedDate BETWEEN @WorkingFromDate AND @WorkingToDate
			) PMPR
		WHERE pms.MemberId = @EmployeeId
	END
	ELSE
	BEGIN
		IF EXISTS (SELECT 1 FROM dbo.EmployeeFlowSetting efs WITH (NOLOCK) WHERE efs.EmployeeId = @EmployeeId)
		BEGIN
			SELECT e.EmployeeId
				,e.NickName
				,e.FullName
				,COUNT(pr.PerformanceRecordId) Quantity
			FROM dbo.EmployeeFlowSetting efs WITH (NOLOCK)
				INNER JOIN dbo.Employee e WITH (NOLOCK) ON efs.EvaluatorId = e.EmployeeId AND efs.ProcessId = @ProcessId AND efs.EmployeeId = @EmployeeId
				LEFT JOIN PerformanceRecord pr WITH (NOLOCK) ON efs.EvaluatorId = pr.CreatorId
					AND pr.EmployeeId = @EmployeeId AND pr.TypeId = 1 AND pr.RoleId = @ProcessId
					AND pr.CreatedDate BETWEEN @WorkingFromDate AND @WorkingToDate
			GROUP BY e.EmployeeId, e.NickName, e.FullName
		END
		ELSE
		BEGIN
			DECLARE @GroupId INT
			SELECT @GroupId = GroupId
			FROM dbo.Employee e WITH (NOLOCK)
			WHERE e.EmployeeId = @EmployeeId

			SELECT e.EmployeeId
				,e.NickName
				,e.FullName
				,COUNT(pr.PerformanceRecordId) Quantity
			FROM dbo.GroupFlowSetting gfs WITH (NOLOCK)
				INNER JOIN dbo.Employee e WITH (NOLOCK) ON gfs.EvaluatorId = e.EmployeeId AND gfs.ProcessId = @ProcessId AND gfs.GroupId = @GroupId
				LEFT JOIN PerformanceRecord pr WITH (NOLOCK) ON gfs.EvaluatorId = pr.CreatorId
					AND pr.EmployeeId = @EmployeeId AND pr.TypeId = 1 AND pr.RoleId = @ProcessId
					AND pr.CreatedDate BETWEEN @WorkingFromDate AND @WorkingToDate
			GROUP BY e.EmployeeId, e.NickName, e.FullName
		END
	END
END
