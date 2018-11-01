
CREATE FUNCTION [dbo].[func_GetMember] (@ActorId INT)
RETURNS @result TABLE (
	EmployeeId INT
	,ProcessId TINYINT
	,RoleId TINYINT
	)
AS
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@PeriodId INT
		,@StartDate DATE
		,@EndDate DATE
		,@SemiTargetId TINYINT = 2
		,@PARejected TINYINT = 5
		-- role
		,@SARoleId TINYINT = 1
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorRoleId TINYINT = 4
		,@ReviewerRoleId TINYINT = 5
		,@PMRoleId TINYINT = 6
		-- process
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId = @SARoleId
			WHERE emp.EmployeeId = @ActorId
			)
	BEGIN
		INSERT INTO @result (
			EmployeeId
			,RoleId
			)
		SELECT EmployeeId
			,@SARoleId
		FROM dbo.Employee WITH (NOLOCK)
		WHERE EmployeeId <> @ActorId
			AND [Enabled] = 1
			AND IsDeleted = 0
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId = @ManagementRoleId
			WHERE emp.EmployeeId = @ActorId
			)
	BEGIN
		INSERT INTO @result (
			EmployeeId
			,RoleId
			)
		SELECT EmployeeId
			,@ManagementRoleId
		FROM dbo.Employee WITH (NOLOCK)
		WHERE EmployeeId <> @ActorId
			AND [Enabled] = 1
			AND IsDeleted = 0
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE TargetTypeId = @SemiTargetId
				AND @Today BETWEEN DisciplineStartDate
					AND DisciplineEndDate
			)
	BEGIN
		SELECT TOP 1 @PeriodId = PeriodId
			,@StartDate = DisciplineStartDate
			,@EndDate = DisciplineEndDate
		FROM dbo.Period WITH (NOLOCK)
		WHERE @Today BETWEEN DisciplineStartDate
				AND DisciplineEndDate
		ORDER BY DisciplineStartDate DESC

		INSERT INTO @result
		SELECT pa.EmployeeId
			,process.ProcessId
			,CASE 
				WHEN process.ProcessId = @EvaluatorProcessId THEN @EvaluatorRoleId
				ELSE @ReviewerRoleId
				END
		FROM dbo.PARecord AS pa WITH (NOLOCK)
			INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
			AND process.EvaluatorId = @ActorId
			AND process.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
			AND emp.EmployeeId <> @ActorId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
			AND period.PeriodId = @PeriodId
		WHERE pa.StatusId <> @PARejected
	END
	ELSE
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.Period WITH (NOLOCK)
				WHERE DisciplineEndDate < @Today
				)
		BEGIN
			SELECT @StartDate = DATEADD(DAY, 1, MAX(DisciplineEndDate))
				,@EndDate = GETDATE()
			FROM dbo.Period WITH (NOLOCK)
			WHERE TargetTypeId = @SemiTargetId
				AND DisciplineEndDate < @Today
		END
		ELSE
		BEGIN
			SELECT @StartDate = '2018-06-01'
				,@EndDate = GETDATE()
		END

		INSERT INTO @result
		SELECT emp.EmployeeId
			,efs.ProcessId
			,CASE 
				WHEN efs.ProcessId = @EvaluatorProcessId THEN @EvaluatorRoleId
				ELSE @ReviewerRoleId
				END
		FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON efs.EmployeeId = emp.EmployeeId
			AND emp.EmployeeId <> @ActorId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON efs.FlowTemplateId = ft.FlowTemplateId
			AND ft.TargetTypeId = @SemiTargetId
			AND ft.IsActive = 1
		WHERE efs.EvaluatorId = @ActorId
			AND efs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)

		INSERT INTO @result
		SELECT emp.EmployeeId
			,gfs.ProcessId
			,CASE 
				WHEN gfs.ProcessId = @EvaluatorProcessId THEN @EvaluatorRoleId
				ELSE @ReviewerRoleId
				END
		FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
			AND gr.IsDeleted = 0
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gr.GroupId = emp.GroupId
			AND emp.EmployeeId <> @ActorId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			INNER JOIN dbo.FlowTemplate AS ft WITH (NOLOCK) ON gfs.FlowTemplateId = ft.FlowTemplateId
			AND ft.TargetTypeId = @SemiTargetId
			AND ft.IsActive = 1
		WHERE gfs.EvaluatorId = @ActorId
			AND gfs.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			AND NOT EXISTS (
				SELECT 1
				FROM @result AS result
				WHERE emp.EmployeeId = result.EmployeeId
					AND result.ProcessId = gfs.ProcessId
				)
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId = @PMRoleId
			WHERE emp.EmployeeId = @ActorId
			)
	BEGIN
		INSERT INTO @result
		SELECT DISTINCT emp.EmployeeId
			,@PMProcessId
			,@PMRoleId
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.ProjectMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
			AND member.EmployeeId <> @ActorId
			AND member.RoleId NOT IN (1, 2, 3, 7) -- none member, anonymous, project manager, viewer
			INNER JOIN dbo.ProjectSpentTime AS spenttime WITH (NOLOCK) ON member.ProjectId = spenttime.ProjectId
			AND member.EmployeeId = spenttime.EmployeeId
			AND spenttime.SpentOn BETWEEN @StartDate
				AND @EndDate
			INNER JOIN dbo.ProjectMember AS pm WITH (NOLOCK) ON member.ProjectId = pm.ProjectId
			AND pm.EmployeeId = @ActorId
			AND pm.RoleId = 3 -- project manager
		WHERE emp.[Enabled] = 1
			AND emp.IsDeleted = 0
	END

	RETURN
END
