
CREATE PROCEDURE [dbo].[Period_GetById] @PeriodId INT
AS
/*
Created: 20180802@Elton.Ton
Task: get period by id
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT p.PeriodId
		,p.[Name]
		,p.TargetTypeId
		,rr.value AS TargetType
		,p.[Description]
		,p.StartDate
		,p.EndDate
		,p.DisciplineStartDate
		,p.DisciplineEndDate
		,p.FocusStartDate
		,p.FocusEndDate
		,p.IsActive
	FROM dbo.Period AS p WITH (NOLOCK)
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON p.TargetTypeId = rr.ResourceId
		AND rr.RefName = 'TargetType'
	WHERE p.PeriodId = @PeriodId
END
