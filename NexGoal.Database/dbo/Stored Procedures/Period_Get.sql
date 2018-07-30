
CREATE PROCEDURE Period_Get
	@TargetTypeId INT = NULL
	,@IsActive BIT = NULL
AS
/*
Created: 20180727@Elton Ton
Task: Create period
DB: NexGoal
*/
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT p.PeriodId
	,p.Name
	,p.TargetTypeId
	,rr.value AS TargetType
	,p.Description
	,p.StartDate
	,p.EndDate
	,p.DisciplineStartDate
	,p.DisciplineEndDate
	,p.FocusStartDate
	,p.IsActive
	FROM dbo.Period p WITH(NOLOCK)
	INNER JOIN dbo.RefResource rr WITH(NOLOCK) ON p.TargetTypeId = rr.ResourceId AND rr.RefName='PeriodTarget'
	WHERE (@TargetTypeId IS NULL OR p.TargetTypeId = @TargetTypeId)
	AND (@IsActive IS NULL OR p.IsActive = @IsActive)
	ORDER BY p.StartDate DESC
END