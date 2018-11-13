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
	SELECT 1 AS IsNominator
	FROM [dbo].[EOQYNominatorSetting] AS setting WITH (NOLOCK)
	WHERE setting.NominatorId = @ActorId
		AND (
			setting.RoleIds = '1'
			OR setting.RoleIds LIKE '1,%'
			OR setting.RoleIds LIKE '%,1'
			)
END
