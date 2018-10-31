
CREATE PROCEDURE [dbo].[Nomination_GetPeriodsForNominator] 
	@NominatorId INT
AS
/*
Created: 20180921@Elton.Ton
Task: get nomination period.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @curentDate DATE = GETDATE()

	SELECT eoqy.EOQYId AS 'NominationPeriodId'
		,eoqy.Quarter
		,eoqy.Year
	FROM dbo.EOQY eoqy WITH (NOLOCK)
	WHERE @curentDate BETWEEN eoqy.StartDateForNominator AND eoqy.EndDateForNominator
		AND EXISTS (SELECT 1 
					FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
					WHERE eoqy.EOQYId = ens.EOQYId AND ens.NominatorId = @NominatorId
						AND EXISTS (SELECT 1 FROM dbo.func_SplitString(ens.RoleIds, ',') r WHERE r.val = 1)
					)
	
END