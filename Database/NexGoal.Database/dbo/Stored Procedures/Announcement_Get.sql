
CREATE PROCEDURE [dbo].[Announcement_Get] @EmployeeId INT
AS
/*
Created: 20181221@elton.ton
Task: get announcements
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @today DATE = GETDATE()
		,@dateFormat VARCHAR(11) = 'dd MMM yyyy'
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@NominatorRoleId TINYINT = 1
		,@CommitteeRoleId TINYINT = 2

	CREATE TABLE #tblAnnouncement (
		[Message] NVARCHAR(MAX)
		,StartDate DATE
		)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The Performance Appraisal Process for the ', p.[Name], ' will begin on ', FORMAT(p.StartDate, @dateFormat), ', you are recommended to create a neat & clean Performance Record (a detailed comment & selected grade) for a better evaluation result.')
		,DATEADD(DAY, - 7, p.StartDate)
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today >= DATEADD(DAY, - 7, p.StartDate)
		AND @today < p.StartDate

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The Performance Appraisal Process for the ', p.[Name], ' is IN PROGRESS, please revise and submit the PA for your members.')
		,p.StartDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today BETWEEN p.StartDate
			AND p.EndDate
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
				INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
			WHERE par.PeriodId = p.PeriodId
				AND pap.ProcessId = @ReviewerProcessId
				AND pap.EvaluatorId = @EmployeeId
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The PA Focus for the ', p.[Name], ' is IN PROGRESS, please set the objectives for your members.')
		,p.FocusStartDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today BETWEEN p.FocusStartDate
			AND p.FocusEndDate
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
				INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
			WHERE par.PeriodId = p.PeriodId
				AND pap.ProcessId = @EvaluatorProcessId
				AND pap.EvaluatorId = @EmployeeId
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT (
			'The Nomination ', CASE e.[Quarter]
			WHEN 1 THEN CONCAT ('Employee of the Quarter - I ', e.[Year])
			WHEN 2 THEN CONCAT ('Employee of the Quarter - II ', e.[Year])
			WHEN 3 THEN CONCAT ('Employee of the Quarter - III ', e.[Year])
			WHEN 4 THEN CONCAT ('Employee of the Quarter - IV ', e.[Year])
			ELSE CONCAT ('Employee of the Year - ', e.[Year]) END, ' process is IN PROGRESS, please submit the vote for your nominee.'
			)
		,e.StartDateForNominator
	FROM dbo.EOQY AS e WITH (NOLOCK)
	WHERE @today BETWEEN e.StartDateForNominator
			AND e.EndDateForNominator
		AND EXISTS (
			SELECT 1
			FROM dbo.EOQYNominatorSetting AS ens WITH (NOLOCK)
			WHERE e.EOQYId = ens.EOQYId
				AND ens.NominatorId = @EmployeeId
				AND EXISTS (
					SELECT 1
					FROM dbo.func_SplitString(ens.RoleIds, ',') r
					WHERE r.val = @NominatorRoleId
					)
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT (
			'The Nomination process for the ', CASE e.[Quarter]
			WHEN 1 THEN CONCAT ('Employee of the Quarter - I ', e.[Year])
			WHEN 2 THEN CONCAT ('Employee of the Quarter - II ', e.[Year])
			WHEN 3 THEN CONCAT ('Employee of the Quarter - III ', e.[Year])
			WHEN 4 THEN CONCAT ('Employee of the Quarter - IV ', e.[Year])
			ELSE CONCAT ('Employee of the Year - ', e.[Year]) END, ' is complete, please review the candidate list and choose the winner(s) for this award.'
			)
		,DATEADD(DAY, 1, e.EndDateForNominator)
	FROM dbo.EOQY AS e WITH (NOLOCK)
	WHERE @today > e.EndDateForNominator
		AND @today <= e.EndDateForCommittee
		AND EXISTS (
			SELECT 1
			FROM dbo.EOQYNominatorSetting AS ens WITH (NOLOCK)
			WHERE e.EOQYId = ens.EOQYId
				AND ens.NominatorId = @EmployeeId
				AND EXISTS (
					SELECT 1
					FROM dbo.func_SplitString(ens.RoleIds, ',') r
					WHERE r.val = @CommitteeRoleId
					)
			)

	DECLARE @NominationPeriodId INT
		,@Quarter TINYINT
		,@Year INT
		,@EndDateForCommittee DATE

	SELECT TOP 1 @NominationPeriodId = e.EOQYId
		,@Quarter = e.[Quarter]
		,@Year = e.[Year]
		,@EndDateForCommittee = e.EndDateForCommittee
	FROM dbo.EOQY AS e WITH (NOLOCK)
	WHERE e.EndDateForCommittee < @today
	ORDER BY e.EndDateForCommittee DESC

	IF @NominationPeriodId IS NOT NULL
	BEGIN
		DECLARE @Winners NVARCHAR(MAX) = STUFF((
					SELECT ', ' + e.NickName + ' - ' + e.FullName
					FROM dbo.Nomination AS n WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON n.NomineeId = e.EmployeeId
					WHERE n.EOQYId = @NominationPeriodId
						AND n.PerformanceRecordId IS NOT NULL
						AND n.StatusId = 3
					FOR XML PATH('')
					), 1, 2, '')

		IF @Winners IS NOT NULL
		BEGIN
			DECLARE @Title VARCHAR(200)

			SELECT @Title = CASE @Quarter
					WHEN 1 THEN CONCAT ('Employee of the Quarter - I ', @Year)
					WHEN 2 THEN CONCAT ('Employee of the Quarter - II ', @Year)
					WHEN 3 THEN CONCAT ('Employee of the Quarter - III ', @Year)
					WHEN 4 THEN CONCAT ('Employee of the Quarter - IV ', @Year)
					ELSE CONCAT ('Employee of the Year - ', @Year)
					END

			INSERT INTO #tblAnnouncement (
				[Message]
				,StartDate
				)
			VALUES (
				CONCAT ('The winners of the ', @Title, ': ', @Winners, '.')
				,DATEADD(DAY, 1, @EndDateForCommittee)
				)
		END
	END

	SELECT [Message]
	FROM #tblAnnouncement WITH (NOLOCK)
	ORDER BY StartDate DESC

	DROP TABLE #tblAnnouncement
END

