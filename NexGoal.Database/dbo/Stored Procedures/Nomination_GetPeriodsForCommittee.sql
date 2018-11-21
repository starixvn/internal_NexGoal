CREATE PROCEDURE [dbo].[Nomination_GetPeriodsForCommittee] 
	@CommitteeId INT
AS
/*
Created: 20180921@Elton.Ton
Task: get nomination period.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT eoqy.EOQYId AS 'NominationPeriodId'
		,eoqy.Quarter
		,eoqy.Year
		,eoqy.StartDateForNominator
		,eoqy.EndDateForNominator
		,eoqy.RemindDateForNominator
		,eoqy.EndDateForCommittee
	FROM dbo.EOQY eoqy WITH (NOLOCK)
	WHERE  EXISTS (SELECT 1 
					FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
					WHERE eoqy.EOQYId = ens.EOQYId AND ens.NominatorId = @CommitteeId
						AND EXISTS (SELECT 1 FROM dbo.func_SplitString(ens.RoleIds, ',') r WHERE r.val = 2)
					)
	