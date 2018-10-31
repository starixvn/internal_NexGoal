
CREATE PROCEDURE [dbo].[Period_Get] @IsActive BIT = NULL
AS
/*
Created: 20180727@Elton.Ton
Task: get period
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Closed TINYINT = 4
		,@Rejected TINYINT = 5

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
		,p.IsActive
		,(
			SELECT COUNT(*)
			FROM dbo.PARecord AS par WITH (NOLOCK)
			WHERE par.PeriodId = p.PeriodId
				AND par.StatusId <> @Rejected
			) AS 'TotalEmployees'
		,(
			SELECT COUNT(*)
			FROM dbo.PARecord AS par WITH (NOLOCK)
			WHERE par.PeriodId = p.PeriodId
				AND par.StatusId NOT IN (@Closed, @Rejected)
			) AS 'TotalUnfinished'
	FROM dbo.Period AS p WITH (NOLOCK)
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON p.TargetTypeId = rr.ResourceId
		AND rr.RefName = 'TargetType'
	WHERE ISNULL(@IsActive, p.IsActive) = p.IsActive
	ORDER BY p.StartDate DESC
END