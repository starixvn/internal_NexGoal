
/****** Object:  Stored Procedure dbo.EPA_GetInfoByEmployee    Script Date: 7/21/2016 2:45:44 PM ******/
CREATE PROCEDURE [dbo].[EPA_GetInfoByEmployee] @EPAId INT
	,@EmployeeId INT
AS
/*
Created: 20150811@Tim.Trinh
Task: Get EPA info.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds IntId
		,@Processes ProcessTable

	INSERT INTO @EPAIds (Id)
	VALUES (@EPAId)

	INSERT INTO @Processes (
		ProcessId
		,EvaluatorId
		)
	VALUES (
		2
		,@EmployeeId
		)

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds

	-- KPI Group
	EXECUTE dbo.EPA_GetKPIGroup @EPAIds

	-- KPI
	EXECUTE dbo.EPA_GetKPI @EPAIds

	-- Evaluation per KPI
	EXECUTE EPA_GetEvaluation @EPAIds
		,@Processes
END
