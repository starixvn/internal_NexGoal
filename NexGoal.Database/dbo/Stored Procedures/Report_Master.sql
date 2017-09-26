
CREATE PROCEDURE [dbo].[Report_Master] @PeriodId INT
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
		,@EOQYs INTID

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO @Groups (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@GroupIds, ',');
	END;

	INSERT INTO @EOQYs
	SELECT DISTINCT nt.EOQYId
	FROM dbo.EOQYNomination AS nt WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON nt.EOQYId = eoq.EOQYId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.AttendanceStartDate <= eoq.EndDate
		AND p.AttendanceEndDate >= eoq.EndDate
	WHERE eoq.StartDate <= CAST(GETDATE() AS DATE)
		AND eoq.EndDate <= CAST(GETDATE() AS DATE)
		AND p.PeriodId = @PeriodId
		AND eoq.IsQuarter = 1
	
	UNION
	SELECT DISTINCT nt.EOQYId
	FROM dbo.EOQYNomination AS nt WITH (NOLOCK)
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON nt.EOQYId = eoq.EOQYId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON 1 = 1
	WHERE eoq.StartDate <= CAST(GETDATE() AS DATE)
		AND eoq.EndDate <= CAST(GETDATE() AS DATE)
		AND p.PeriodId = @PeriodId
		AND eoq.IsQuarter = 0
		AND YEAR(eoq.StartDate) IN (Year(p.StartDate), Year(p.StartDate) - 1)

	SELECT DISTINCT epaReviewer.AdjustScore AS ReviewerAdjustment
		,f.PAFocusId
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
	FROM dbo.PAFocus AS f WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON f.EmployeeId = e.EmployeeId
		INNER JOIN dbo.HRMJobTitle AS title WITH (NOLOCK) ON title.Id = e.JobTitle
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = f.PeriodId
		INNER JOIN dbo.PAFocusProcess AS pc WITH (NOLOCK) ON pc.PAFocusId = f.PAFocusId
		LEFT JOIN dbo.EPA AS epa WITH (NOLOCK) ON epa.EmployeeId = e.EmployeeId
		AND epa.PeriodId = p.PeriodId
		LEFT JOIN dbo.ObjectiveEmployee AS l WITH (NOLOCK) ON l.ObjectiveId = f.LastObjectiveId
		LEFT JOIN dbo.ObjectiveEmployee AS n WITH (NOLOCK) ON n.ObjectiveId = f.NextObjectiveId
		LEFT JOIN (
		SELECT AdjustScore
			,epa.EmployeeId
			,epa.PeriodId
		FROM dbo.EPA AS epa WITH (NOLOCK)
			INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON pc.EPAId = epa.EPAId
		WHERE pc.ProcessId = 4
		) AS epaReviewer ON epaReviewer.EmployeeId = e.EmployeeId
		AND epaReviewer.PeriodId = p.PeriodId
	WHERE p.PeriodId = @PeriodId
		AND p.PAFocusEndDate < CONVERT(DATE, getdate())
		AND e.IsDeleted = 0
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
	ORDER BY g.GroupId

	SELECT epa.EmployeeId
		,epa.PeriodId
		,pc.Comment
		,e.NickName AS EvaluatorNickName
		,e.FullName AS EvaluatorFullName
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON pc.EPAId = epa.EPAId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = pc.EvaluatorId
	WHERE pc.ProcessId = 3
		AND epa.PeriodId = @PeriodId
		AND (
			@EmployeeId IS NULL
			OR epa.EmployeeId = @EmployeeId
			)
		AND pc.Comment IS NOT NULL
		AND pc.Comment <> ''

	SELECT DISTINCT nomi.NomineeId
		,nomi.StatusId
		,ISNULL(award.[Quarter], 0) AS [Quarter]
		,award.[Year]
	FROM dbo.EOQY AS award WITH (NOLOCK)
		INNER JOIN @EOQYs AS award1 ON award1.Id = award.EOQYId
		INNER JOIN dbo.EOQYNomination AS nomi WITH (NOLOCK) ON nomi.EOQYId = award.EOQYId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = nomi.NomineeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
	WHERE (
			@EmployeeId IS NULL
			OR nomi.NomineeId = @EmployeeId
			)
		AND (
			@GroupIds IS NULL
			OR g.GroupId IN (
				SELECT REPLACE(Id, '-', '')
				FROM @Groups
				WHERE Id LIKE '-%'
				)
			)
	ORDER BY ISNULL(award.[Quarter], 0)

	SELECT DISTINCT nomi.NomineeId
		,nomi.NominatorId
		,nominator.NickName AS NominatorNickName
		,nominator.FullName AS NominatorFullName
	FROM @EOQYs AS award
		INNER JOIN dbo.EOQYNomination AS nomi WITH (NOLOCK) ON nomi.EOQYId = award.Id
		INNER JOIN dbo.Employee AS nominator WITH (NOLOCK) ON nominator.EmployeeId = nomi.NominatorId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = nomi.NomineeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
	WHERE (
			@EmployeeId IS NULL
			OR nomi.NomineeId = @EmployeeId
			)
		AND (
			@GroupIds IS NULL
			OR g.GroupId IN (
				SELECT REPLACE(Id, '-', '')
				FROM @Groups
				WHERE Id LIKE '-%'
				)
			)
END