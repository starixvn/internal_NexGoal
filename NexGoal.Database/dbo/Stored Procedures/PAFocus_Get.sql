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
    20170927@Tim.Trinh: evaluator/reviewer can view the process of that period anytime instead of waiting until their period
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Groups IntId
		,@Is_MGT_SA BIT
		,@PAFocusIds IntId
		,@Today DATE = GETDATE()

	IF @GroupIds IS NOT NULL
	BEGIN
		INSERT INTO @Groups (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@GroupIds, ',');
	END;

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = e.EmployeeId
				INNER JOIN dbo.RoleFunction AS r WITH (NOLOCK) ON r.RoleId = er.RoleId
				INNER JOIN dbo.FunctionKey AS f WITH (NOLOCK) ON f.FunctionId = r.FunctionId
			WHERE f.FunctionKey = 'PAFocus'
				AND r.IsRead = 1
				AND e.[Enabled] = 1
				AND e.IsDeleted = 0
				AND e.EmployeeId = @EvaluatorId
				AND er.RoleId = 3 --managment
			
			UNION
			SELECT 1
			FROM dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = e.EmployeeId
			WHERE e.[Enabled] = 1
				AND e.IsDeleted = 0
				AND e.EmployeeId = @EvaluatorId
				AND er.RoleId = 1 --SA
			)
	BEGIN
		SET @Is_MGT_SA = 1
	END

	IF (@Is_MGT_SA = 1)
	BEGIN
		INSERT INTO @PAFocusIds (Id)
		SELECT DISTINCT f.PAFocusId
		FROM dbo.PAFocus AS f WITH (NOLOCK)
			INNER JOIN dbo.PAFocusProcess AS pc WITH (NOLOCK) ON pc.PAFocusId = f.PAFocusId
			INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = f.PeriodId
		WHERE p.PeriodId = @PeriodId
			AND pc.StartDate <= @Today
			AND pc.ProcessId = 3
	END

	INSERT INTO @PAFocusIds (Id)
	SELECT DISTINCT f.PAFocusId
	FROM dbo.PAFocus AS f WITH (NOLOCK)
		INNER JOIN dbo.PAFocusProcess AS pc WITH (NOLOCK) ON pc.PAFocusId = f.PAFocusId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = f.PeriodId
	WHERE pc.EvaluatorId = @EvaluatorId
		AND p.PeriodId = @PeriodId
		AND pc.StartDate <= @Today
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
		,title.[Name] AS JobTitleName
		,g.GroupId
		,g.[Name] AS GroupName
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
		AND ISNULL(@EmployeeId, e.EmployeeId) = e.EmployeeId
		AND (
			@GroupIds IS NULL
			OR g.GroupId IN (
				SELECT REPLACE(Id, '-', '')
				FROM @Groups
				WHERE Id LIKE '-%'
				)
			)
	ORDER BY g.[Name]
		,e.EmployeeId
END