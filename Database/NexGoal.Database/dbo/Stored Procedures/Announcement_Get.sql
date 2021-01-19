

CREATE PROCEDURE [dbo].[Announcement_Get] @EmployeeId INT
AS
/*
Created: 20181221@elton.ton
Task: get announcements
DB: bodb_NexGoal
Original:

Revisions:
	20190411@elton.ton: change date to send notifications before period start date
	20200226@drake.doi: get announcement for probation
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @today DATE = GETDATE()
		,@dateFormat VARCHAR(11) = 'dd MMM yyyy'
		,@PARejected TINYINT = 5
		,@EvaluatorProcessId TINYINT = 4
		,@SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		,@NominatorRoleId TINYINT = 1
		,@CommitteeRoleId TINYINT = 2
		,@SystemRole_HR TINYINT = 2

	CREATE TABLE #tblAnnouncement (
		[Message] NVARCHAR(MAX)
		,StartDate DATE
		)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The NexGoal system will capture all the performance records on ', FORMAT(p.StartDate, @dateFormat), '. You are recommended to create a neat & clean Performance Record for yourself BEFORE ', FORMAT(p.StartDate, @dateFormat), '. Please be advised that any record created after that time will NOT be valid for this PA period.')
		,DATEADD(DAY, - 28, p.StartDate)
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today >= DATEADD(DAY, - 28, p.StartDate)
		AND @today < p.StartDate
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
				WHERE efs.EmployeeId = @EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON gfs.GroupId = e.GroupId
					AND e.EmployeeId = @EmployeeId
				)
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The NexGoal system will capture all the performance records on ', FORMAT(p.StartDate, @dateFormat), '. You are recommended to create a Performance Record for your members (with a detailed comment and also selected grade) BEFORE ', FORMAT(p.StartDate, @dateFormat), '. Please be advised that any record created after that time will NOT be valid for this PA period.')
		,DATEADD(DAY, - 7, p.StartDate)
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today >= DATEADD(DAY, - 7, p.StartDate)
		AND @today < p.StartDate
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
				WHERE efs.EvaluatorId = @EmployeeId
					AND efs.ProcessId > @SelfProcessId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
				WHERE gfs.EvaluatorId = @EmployeeId
					AND gfs.ProcessId > @SelfProcessId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.PMSetting AS pmsetting WITH (NOLOCK)
				WHERE pmsetting.ProjectManagerId = @EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.PAProcess AS pap WITH (NOLOCK)
					INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
				WHERE par.PeriodId = p.PeriodId
					AND par.StatusId != @PARejected
					AND pap.ProcessId > @SelfProcessId
					AND pap.EvaluatorId = @EmployeeId
				)
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The Performance Appraisal Process for the ', p.[Name], ' is IN PROGRESS and will end at 23:59 ', FORMAT(p.EndDate, @dateFormat), ', please revise and submit the PA for your members.')
		,p.StartDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today BETWEEN p.StartDate
			AND p.EndDate
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
				INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
			WHERE par.PeriodId = p.PeriodId
				AND par.StatusId != @PARejected
				AND pap.ProcessId = @ReviewerProcessId
				AND pap.EvaluatorId = @EmployeeId
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The PA Focus for the ', p.[Name], ' is IN PROGRESS and will end at 23:59 ', FORMAT(p.FocusEndDate, @dateFormat), ', please set the objectives for your members.')
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
			ELSE CONCAT ('Employee of the Year - ', e.[Year]) END, ' process is IN PROGRESS and will end at 23:59 ', FORMAT(e.EndDateForNominator, @dateFormat), ', please submit the vote for your nominee.'
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


	DECLARE @EmployeesInProbation NVARCHAR(200) = NULL

	SELECT @EmployeesInProbation = CONCAT(COALESCE(@EmployeesInProbation + ', ', ''), e.NickName)
	FROM ProbationEvaluation AS p WITH (NOLOCK)
		INNER JOIN Employee AS e WITH (NOLOCK) ON p.EmployeeId = e.EmployeeId
		INNER JOIN ProbationSetting  AS ps WITH (NOLOCK) ON p.EmployeeId = ps.EmployeeId AND ps.ProcessId = @EvaluatorProcessId
	WHERE e.JoinedDate = @today AND e.EmployeeId IS NOT NULL
		AND (
			EXISTS (
			SELECT 1
			FROM EmployeeRole AS er WITH (NOLOCK)
				INNER JOIN RefResource AS ref WITH (NOLOCK) 
					ON ref.RefName = 'SystemRole' AND ref.Value = 'HR' AND er.RoleId = ref.ResourceId
			WHERE er.EmployeeId = @EmployeeId
			)
			OR ps.EvaluatorId = @EmployeeId
		)

	IF(@EmployeesInProbation IS NOT NULL)
	BEGIN
		INSERT INTO #tblAnnouncement ([Message], StartDate)
			(SELECT CONCAT('The Probation evaluation process for ', @EmployeesInProbation, ' has been created on the system. Please visit the NexGoal site to review the Probation settings.'), @today) 
	END

	DECLARE @EmployeesStartReviewProbation NVARCHAR(200) = NULL

	SELECT @EmployeesStartReviewProbation = CONCAT(COALESCE(@EmployeesStartReviewProbation + ', ', ''), e.NickName)
	FROM ProbationEvaluation AS p WITH (NOLOCK)
		INNER JOIN Employee AS e WITH (NOLOCK) ON p.EmployeeId = e.EmployeeId
		INNER JOIN ProbationSetting  AS ps WITH (NOLOCK) ON p.EmployeeId = ps.EmployeeId
	WHERE ps.ProcessId = @ReviewerProcessId
		AND ps.FromDate = @today
		AND ps.EvaluatorId = @EmployeeId

	IF(@EmployeesStartReviewProbation IS NOT NULL)
	BEGIN
		INSERT INTO #tblAnnouncement ([Message], StartDate)
			(SELECT CONCAT('Your evaluation period for ', @EmployeesStartReviewProbation, ' has begun. To conduct a member assessment, please visit the NexGoal site and complete the evaluation form(s).'), @today) 
	END

	INSERT INTO #tblAnnouncement ([Message], StartDate)
		SELECT CONCAT('Your evaluation period will end at 23:59, ', ps.ToDate,'. Please visit the NexGoal site to complete the evaluation form.')
			,ps.ToDate
		FROM ProbationSetting AS ps WITH (NOLOCK)
		WHERE ps.ProcessId = @SelfProcessId
			AND ps.EmployeeId = @EmployeeId
			AND ps.FromDate =  @today
	
	SELECT [Message]
	FROM #tblAnnouncement WITH (NOLOCK)
	ORDER BY StartDate DESC

	DROP TABLE #tblAnnouncement
END