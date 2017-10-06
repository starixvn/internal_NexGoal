CREATE PROCEDURE [dbo].[EOQ_GetLatestNominationByNominatorId] @NominatorId INT
AS
/*
Created: 20160907@Tim.Trinh
Task: Get current nomination by NominatorId.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@NominatorRole TINYINT = 1
		,@CommitteeRole TINYINT = 2

	CREATE TABLE #tblNominatorSetting (
		Id INT
		,RoleId TINYINT
		)

	INSERT INTO #tblNominatorSetting (
		Id
		,RoleId
		)
	SELECT DISTINCT data.EOQYId
		,f.val
	FROM (
		SELECT TOP 1 setting.RoleId
			,eoq.EOQYId
		FROM dbo.EOQY AS eoq WITH (NOLOCK)
			INNER JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON eoq.EOQYId = setting.EOQYId
		WHERE setting.NominatorId = @NominatorId
			AND eoq.IsQuarter = 1
			AND eoq.StartDate <= @Today
		ORDER BY eoq.EndDate DESC
		) AS data
		CROSS APPLY dbo.fnc_strSplit(data.RoleId, ',') AS f

	IF (
			SELECT COUNT(1)
			FROM #tblNominatorSetting WITH (NOLOCK)
			) = 2
	BEGIN
		SELECT (CASE 
				WHEN @Today BETWEEN eoq.StartDate
						AND eoq.EndDate THEN @NominatorRole
				ELSE @CommitteeRole END
				) AS RoleIds
			,eoq.EOQYId
			,eoq.[Quarter]
			,eoq.[Year]
			,eoq.StartDate
			,eoq.EndDate
			,eoq.RemindDate
			,eoq.EndDateForCommittee
		FROM dbo.EOQY AS eoq WITH (NOLOCK)
			INNER JOIN #tblNominatorSetting AS setting WITH (NOLOCK) ON eoq.EOQYId = setting.Id
	END
	ELSE
	BEGIN
		SELECT setting.RoleId AS RoleIds
			,eoq.EOQYId
			,eoq.[Quarter]
			,eoq.[Year]
			,eoq.StartDate
			,eoq.EndDate
			,eoq.RemindDate
			,eoq.EndDateForCommittee
		FROM dbo.EOQY AS eoq WITH (NOLOCK)
			INNER JOIN #tblNominatorSetting AS setting WITH (NOLOCK) ON eoq.EOQYId = setting.Id
	END
	
	DROP TABLE #tblNominatorSetting
END