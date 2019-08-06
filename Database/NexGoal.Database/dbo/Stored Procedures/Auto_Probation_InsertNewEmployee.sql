CREATE PROCEDURE [dbo].[Auto_Probation_InsertNewEmployee]
	@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190520@Elton.ton
Task: add new employees to probation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@EmployeeIds VARCHAR(MAX)

	SET @EmployeeIds= STUFF(
		(SELECT CONCAT(',', e.EmployeeId)
		FROM dbo.Employee e WITH (NOLOCK)		
		WHERE e.JoinedDate BETWEEN DATEADD(MONTH, -2, @Today) AND @Today AND e.ConfirmedDate IS NULL 
			AND IsDeleted = 0
			AND NOT EXISTS (SELECT 1 FROM dbo.ProbationEvaluation pe WITH (NOLOCK) WHERE pe.EmployeeId = e.EmployeeId)
		FOR XML PATH('')), 1, 1, '')

	IF @EmployeeIds IS NOT NULL
	BEGIN
		exec dbo.Probation_InsertNewEmployees @EmployeeIds
	END
END