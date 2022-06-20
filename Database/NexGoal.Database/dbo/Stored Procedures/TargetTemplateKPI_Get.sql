CREATE PROCEDURE [dbo].[TargetTemplateKPI_Get]

AS
/*
Created: 20220610@Otis.Vu
Task: Get target template kpi by targetTeamplateId
DB: bodb_NexGoal
Original:

Revisions:
*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TargetTemplateId INT

	SELECT TOP 1 @TargetTemplateId = TargetTemplateId
	FROM dbo.TargetTemplate WITH (NOLOCK)
	WHERE TargetTypeId = 4 -- MOEY
		AND IsActive = 1

	SELECT kpig.KPIGroupId, kpig.Name, kpi.KPIId, kpi.Name AS KPIName
	FROM KPIGroup AS kpig WITH (NOLOCK)
		inner join KPI AS kpi WITH (NOLOCK) ON kpig.KPIGroupId = kpi.KPIGroupId
		inner join TargetTemplateKPIGroup AS ttkg WITH (NOLOCK) ON kpig.KPIGroupId = ttkg.KPIGroupId
		inner join TargetTemplateKPI AS ttk WITH (NOLOCK) ON kpi.KPIId = ttk.KPIId and kpi.KPIGroupId = ttk.KPIGroupId
		inner join TargetTemplate AS tt WITH (NOLOCK) ON tt.TargetTemplateId = ttkg.TargetTemplateId and tt.TargetTemplateId = ttk.TargetTemplateId
	WHERE tt.TargetTemplateId = @TargetTemplateId
	Order By ttkg.DisplayOrder

END