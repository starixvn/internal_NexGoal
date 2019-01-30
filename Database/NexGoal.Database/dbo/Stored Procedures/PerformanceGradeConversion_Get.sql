CREATE PROCEDURE [dbo].[PerformanceGradeConversion_Get]
AS
/*
Created: 20190130@Elton.Ton
Task: get grade conversion
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT GradeId
		,Point
	FROM dbo.PerformanceGradeConversion WITH (NOLOCK)
END

