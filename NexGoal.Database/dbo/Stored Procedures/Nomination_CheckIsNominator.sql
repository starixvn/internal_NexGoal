CREATE PROCEDURE [dbo].[Nomination_CheckIsNominator] @ActorId INT
AS
/*
Created: 201801107@Oliver.Dang
Task: check if nominator.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@LastestEOYQ INT

	SELECT TOP 1 @LastestEOYQ = EOQYId
	FROM [dbo].[EOQY]
	ORDER BY StartDateForNominator DESC

	SELECT 1 AS IsNominator
	FROM [dbo].[EOQYNominatorSetting] AS setting WITH (NOLOCK)
	WHERE setting.EOQYId = @LastestEOYQ
		AND setting.NominatorId = @ActorId
		AND (
			setting.RoleIds = '1'
			OR setting.RoleIds LIKE '1,%'
			OR setting.RoleIds LIKE '%,1'
			)
END
