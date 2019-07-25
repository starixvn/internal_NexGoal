
CREATE FUNCTION [dbo].[fnc_CheckIsProbation] (@EmployeeId INT)
RETURNS BIT
AS
/*
Created: 20190308@Tim.Trinh
Task: check member is in probation duration.
DB: bodb_NexGoal
Original:

Revisions:
20190725@Elton.Ton: change with new probation function
Param's Explanation:

*/
BEGIN
	DECLARE @IsProbation BIT = 0
		,@Today DATE = GETDATE()
		,@ProbationDuration SMALLINT = 2 -- month

	IF EXISTS (SELECT 1 FROM dbo.ProbationSetting WITH (NOLOCK) WHERE EmployeeId = @EmployeeId)
	BEGIN
		IF EXISTS (SELECT 1 FROM dbo.ProbationSetting WITH (NOLOCK) WHERE EmployeeId = @EmployeeId AND ProcessId = 5 AND ToDate >= @Today)
		BEGIN
			SET @IsProbation = 1
		END
	END
	ELSE
	BEGIN
		IF EXISTS (
			SELECT 1
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
				AND @Today BETWEEN JoinedDate
					AND DATEADD(MONTH, @ProbationDuration, JoinedDate)
			)
		BEGIN
			SET @IsProbation = 1
		END
	END

	RETURN @IsProbation
END