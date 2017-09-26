


CREATE PROCEDURE [dbo].[PAFocus_Get] @EvaluatorId INT
	,@PeriodId INT
	,@GroupIds NVARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20160830@Razor.Nguyen
Task: PA Focus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Groups INTID
		,@IsManagementSA BIT
		,@PAFocusIds INTID

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO @Groups (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@GroupIds, ',');
	END;

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee e
				INNER JOIN dbo.EmployeeRole er  WITH(NOLOCK)  ON er.EmployeeId = e.EmployeeId
				INNER JOIN dbo.RoleFunction r  WITH(NOLOCK)  ON r.RoleId = er.RoleId
				INNER JOIN dbo.FunctionKey f  WITH(NOLOCK)  ON f.FunctionId = r.FunctionId
			WHERE f.FunctionKey = 'PAFocus'
				AND r.IsRead = 1
				AND e.[Enabled] = 1
				AND e.IsDeleted = 0
				AND e.EmployeeId = @EvaluatorId
				AND er.RoleId = 3 --managment
			
			UNION
			SELECT 1
			FROM dbo.Employee e
				INNER JOIN dbo.EmployeeRole er  WITH(NOLOCK)  ON er.EmployeeId = e.EmployeeId
			WHERE e.[Enabled] = 1
				AND e.IsDeleted = 0
				AND e.EmployeeId = @EvaluatorId
				AND er.RoleId = 1 --SA
			)
	BEGIN
		SET @IsManagementSA = 1
	END

	IF (@IsManagementSA = 1)
	BEGIN
		INSERT INTO @PAFocusIds (Id)
		SELECT DISTINCT f.PAFocusId
		FROM dbo.PAFocus AS f WITH (NOLOCK)
			INNER JOIN dbo.PAFocusProcess AS pc WITH (NOLOCK) ON pc.PAFocusId = f.PAFocusId
			INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = f.PeriodId
		WHERE p.PeriodId = @PeriodId
			AND pc.EndDate < CONVERT(DATE, getdate())
			AND pc.ProcessId = 3
	END

	INSERT INTO @PAFocusIds (Id)
	SELECT DISTINCT f.PAFocusId
	FROM dbo.PAFocus AS f WITH (NOLOCK)
		INNER JOIN dbo.PAFocusProcess AS pc WITH (NOLOCK) ON pc.PAFocusId = f.PAFocusId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = f.PeriodId
	WHERE pc.EvaluatorId = @EvaluatorId
		AND p.PeriodId = @PeriodId
		AND pc.StartDate <= CONVERT(DATE, getdate())
		AND f.PAFocusId NOT IN (
			SELECT Id
			FROM @PAFocusIds
			)

	SELECT DISTINCT f.PAFocusId
		,f.PeriodId
		,l.[Description] AS LastCompletedPercentObjective
		,n.[Description] AS NextDescriptionObjective
		,f.Contribution AS EncryptedContribution
		,f.BonusGiven AS EncryptedBonusGiven
		,f.IsPoorPerformance AS EncryptedIsPoorPerformance
		,f.IsReadyPromotion AS EncryptedIsReadyPromotion
		,epa.FinalScore AS EncryptedFinalScore
		,e.EmployeeId
		,e.NickName
		,e.FullName
		,title.Id AS JobTitleId
		,title.NAME AS JobTitleName
		,g.GroupId
		,g.NAME AS GroupName
	FROM @PAFocusIds AS f1
		INNER JOIN dbo.PAFocus AS f WITH (NOLOCK) ON f.PAFocusId = f1.Id
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON f.EmployeeId = e.EmployeeId
		INNER JOIN dbo.HRMJobTitle AS title WITH (NOLOCK) ON title.Id = e.JobTitle
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = f.PeriodId
		LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON epa.EmployeeId = e.EmployeeId
		AND epa.PeriodId = p.PeriodId
		LEFT JOIN dbo.ObjectiveEmployee AS l WITH (NOLOCK) ON l.ObjectiveId = f.LastObjectiveId
		LEFT JOIN dbo.ObjectiveEmployee AS n WITH (NOLOCK) ON n.ObjectiveId = f.NextObjectiveId
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
		AND (
			@EmployeeId IS NULL
			OR e.EmployeeId = @EmployeeId
			)
		AND (
			@GroupIds IS NULL
			OR g.GroupId IN (
				SELECT REPLACE(Id, '-', '')
				FROM @Groups
				WHERE Id LIKE '-%'
				)
			)
	ORDER BY g.NAME, e.EmployeeId
END