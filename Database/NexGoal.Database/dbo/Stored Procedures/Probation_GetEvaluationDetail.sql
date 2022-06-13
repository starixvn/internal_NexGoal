
CREATE PROCEDURE [dbo].[Probation_GetEvaluationDetail] @EmployeeId INT
AS
/*
Created: 20190520@Elton.ton
Task: get evaluation detail
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

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