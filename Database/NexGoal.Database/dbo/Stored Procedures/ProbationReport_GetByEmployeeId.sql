CREATE PROCEDURE [dbo].[ProbationReport_GetByEmployeeId]
	@EmployeeId INT
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

	DECLARE	@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3

	IF EXISTS (SELECT 1 FROM dbo.func_GetMember(@ActorId) WHERE EmployeeId = @EmployeeId AND ProcessId IN (4,5))
		OR EXISTS (SELECT 1 FROM dbo.EmployeeRole WITH (NOLOCK) WHERE RoleId IN (@HRRoleId, @ManagementRoleId) AND EmployeeId = @ActorId)
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
			,jt.Name AS JobTitle
			,g.Name AS GroupName
			,pe.SelfScore
			,pe.SelfComment
			,pe.EvaluatorScore
			,pe.EvaluatorComment
			,pe.ReviewerComment
			,(CASE 
				WHEN @CanViewEvaluatorResult = 1 THEN pe.IsQualifiedByEvaluator
				ELSE NULL
			END) AS IsQualifiedByEvaluator
			,@CanViewEvaluatorResult AS CanViewEvaluatorResult
			,(
				SELECT e.NickName
				FROM dbo.ProbationSetting ps WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON ps.EvaluatorId = e.EmployeeId
				WHERE ps.EmployeeId = @EmployeeId AND ps.ProcessId = 4
			) AS EvaluatorNickName
			,(
				SELECT e.NickName
				FROM dbo.ProbationSetting ps WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON ps.EvaluatorId = e.EmployeeId
				WHERE ps.EmployeeId = @EmployeeId AND ps.ProcessId = 5
			) AS ReviewerNickName
		FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
			INNER JOIN dbo.Employee e WITH (NOLOCK) ON pe.EmployeeId = e.EmployeeId
			INNER JOIN dbo.JobTitle jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
			INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
		WHERE pe.EmployeeId = @EmployeeId
		AND EXISTS (
			SELECT 1
			FROM dbo.ProbationSetting ps WITH (NOLOCK)
			WHERE ps.EmployeeId = pe.EmployeeId AND ps.ProcessId = 5 AND ps.ToDate < GETDATE()
		)

		SELECT KPIGroupId
			,Name
		FROM dbo.KPIGroup WITH (NOLOCK)

		SELECT ped.EmployeeId
			,ped.KPIId
			,ped.SelfPoint
			,ped.EvaluatorPoint
			,k.Name AS KPIName
			,k.KPIGroupId
		FROM dbo.ProbationEvaluationDetail ped WITH (NOLOCK)
			INNER JOIN dbo.KPI k ON ped.KPIId = k.KPIId
			INNER JOIN dbo.KPIGroup kg ON kg.KPIGroupId = k.KPIGroupId
		WHERE ped.EmployeeId = @EmployeeId
	END
END