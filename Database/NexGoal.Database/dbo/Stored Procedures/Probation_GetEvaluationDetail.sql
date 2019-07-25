CREATE PROCEDURE [dbo].[Probation_GetEvaluationDetail]
	@EmployeeId INT
AS
/*
Created: 20190520@Elton.ton
Task: get evaluation detail
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

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