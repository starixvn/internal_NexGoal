
CREATE PROCEDURE [dbo].[PerformanceGrade_Get]
AS
/*
Created: 20190228@Tim.Trinh
Task: get performance grade
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT GradeId
		,[Name]
		,[Description]
		,Point
	FROM dbo.PerformanceGrade WITH (NOLOCK)
END