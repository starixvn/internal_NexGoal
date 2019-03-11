
CREATE PROCEDURE [dbo].[Project_GetByEmployeeId] @EmployeeId INT
AS
/*
Created: 20190308@Tim.Trinh
Task: Get project by employee
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee
			WHERE EmployeeId = @EmployeeId
				AND GroupId IN (1, 2, 38) -- FA, ITNW, HR
			)
		OR dbo.fnc_CheckIsProbation(@EmployeeId) = 1
	BEGIN
		SELECT ProjectId
			,[Name]
		FROM dbo.Project WITH (NOLOCK)
		WHERE ProjectId < 0
	END
	ELSE
	BEGIN
		SELECT ProjectId
			,[Name]
		FROM dbo.Project WITH (NOLOCK)
	END
END