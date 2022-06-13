
CREATE PROCEDURE [dbo].[ProbationReport_GetByEmployeeId] @EmployeeId INT
	,@ActorId INT
AS
/*
Created: 20190705@Elton.ton
Task: get probationer's evaluation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3

	IF EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@ActorId)
			WHERE EmployeeId = @EmployeeId
				AND ProcessId IN (4, 5)
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole WITH (NOLOCK)
			WHERE RoleId IN (@HRRoleId, @ManagementRoleId)
				AND EmployeeId = @ActorId
			)
		OR @EmployeeId = @ActorId
	BEGIN
		DECLARE @CanViewEvaluatorResult BIT = 0

		IF @EmployeeId != @ActorId
		BEGIN
			SET @CanViewEvaluatorResult = 1
		END

		SELECT pe.EmployeeId
			,pe.IsQualified
			,e.NickName
			,e.FullName
			,e.Code
			,e.GroupId
			,e.JoinedDate
			,jt.[Name] AS JobTitle
			,g.[Name] AS GroupName
			,pe.SelfScore
			,pe.SelfComment
			,pe.EvaluatorScore
			,pe.EvaluatorComment
			,pe.ReviewerComment
			,(CASE 
				WHEN @CanViewEvaluatorResult = 1 THEN pe.IsQualifiedByEvaluator
				ELSE NULL END
				) AS IsQualifiedByEvaluator
			,@CanViewEvaluatorResult AS CanViewEvaluatorResult
			,(
				SELECT e.NickName
				FROM dbo.ProbationSetting ps WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON ps.EvaluatorId = e.EmployeeId
				WHERE ps.EmployeeId = @EmployeeId
					AND ps.ProcessId = 4
				) AS EvaluatorNickName
			,(
				SELECT e.NickName
				FROM dbo.ProbationSetting ps WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON ps.EvaluatorId = e.EmployeeId
				WHERE ps.EmployeeId = @EmployeeId
					AND ps.ProcessId = 5
				) AS ReviewerNickName
		FROM dbo.ProbationEvaluation AS pe WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pe.EmployeeId = e.EmployeeId
			INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
			INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		WHERE pe.EmployeeId = @EmployeeId
			AND EXISTS (
				SELECT 1
				FROM dbo.ProbationSetting AS ps WITH (NOLOCK)
				WHERE ps.EmployeeId = pe.EmployeeId
					AND ps.ProcessId = 5
					AND ps.ToDate < GETDATE()
				)

		DECLARE @TargetTemplateId INT

		SELECT TOP 1 @TargetTemplateId = TargetTemplateId
		FROM dbo.TargetTemplate WITH (NOLOCK)
		WHERE TargetTypeId = 1 -- probation
			AND IsActive = 1

		SELECT kg.KPIGroupId
			,kg.[Name]
		FROM dbo.TargetTemplateKPIGroup AS ttkg WITH (NOLOCK)
			INNER JOIN dbo.KPIGroup AS kg WITH (NOLOCK) ON ttkg.KPIGroupId = kg.KPIGroupId
		WHERE ttkg.TargetTemplateId = @TargetTemplateId
		ORDER BY ttkg.DisplayOrder

		SELECT ped.EmployeeId
			,ped.KPIId
			,ped.SelfPoint
			,ped.EvaluatorPoint
			,k.[Name] AS KPIName
			,k.KPIGroupId
		FROM dbo.ProbationEvaluationDetail AS ped WITH (NOLOCK)
			INNER JOIN dbo.KPI AS k ON ped.KPIId = k.KPIId
			INNER JOIN dbo.TargetTemplateKPI AS ttk ON ttk.KPIGroupId = k.KPIGroupId
			AND k.KPIId = ttk.KPIId
			INNER JOIN dbo.TargetTemplateKPIGroup AS ttkg WITH (NOLOCK) ON ttk.TargetTemplateId = ttkg.TargetTemplateId
			AND ttk.KPIGroupId = ttkg.KPIGroupId
		WHERE ped.EmployeeId = @EmployeeId
			AND ttk.TargetTemplateId = @TargetTemplateId
		ORDER BY ttkg.DisplayOrder, ttk.DisplayOrder
	END
END
