CREATE PROCEDURE [dbo].[Announcement_Get]
	@EmployeeId INT
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
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @today DATE = GETDATE(), @dateFormat VARCHAR(11) = 'dd MMM yyyy'

	CREATE TABLE #announcement ([Message] VARCHAR(MAX))

	INSERT INTO #announcement
	SELECT CONCAT('PA Process will be available from ', FORMAT(p.StartDate, @dateFormat), ' and any change after ', FORMAT(p.EndDate, @dateFormat), ' are not allowed.')
	FROM dbo.Period p WITH (NOLOCK)
	WHERE p.StartDate <= DATEADD(DAY, 7, @today) AND p.EndDate >= @today

	DECLARE @NominationPeriodId INT, @Quarter TINYINT, @Year INT

	SELECT TOP 1  @NominationPeriodId = e.EOQYId
		,@Quarter = e.Quarter
		,@Year = e.Year
	FROM dbo.EOQY e WITH (NOLOCK)
	WHERE e.EndDateForCommittee < @today
	ORDER BY e.EndDateForCommittee

	IF @NominationPeriodId IS NOT NULL
	BEGIN
		DECLARE @Winners VARCHAR(MAX) 
		= STUFF((SELECT ', ' + e.NickName
				FROM dbo.Nomination n WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON n.NomineeId = e.EmployeeId
				WHERE n.EOQYId = @NominationPeriodId AND n.PerformanceRecordId IS NOT NULL AND n.StatusId = 3
				FOR XML PATH('')), 1, 2, '')

		IF @Winners IS NOT NULL
		BEGIN
			DECLARE @Title VARCHAR(200)

			SELECT @Title = CASE @Quarter
								WHEN 1 THEN 'Employee of the Quarter - I'
								WHEN 2 THEN 'Employee of the Quarter - II'
								WHEN 3 THEN 'Employee of the Quarter - III'
								WHEN 4 THEN 'Employee of the Quarter - IV'
								ELSE CONCAT('Employee of the Year - ', @Year)
							END

			INSERT INTO #announcement
			VALUES(CONCAT('Congratulation to the winners of ', @Title, ': ', @Winners, '.'))
		END
	END

	INSERT INTO #announcement
	SELECT CONCAT('PA Focus of ', p.Name,' will be from ', FORMAT(p.FocusStartDate, @dateFormat), ' to ', FORMAT(p.FocusEndDate, @dateFormat), '.')
	FROM dbo.Period p WITH (NOLOCK)
	WHERE @today BETWEEN p.FocusStartDate AND p.FocusEndDate
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess pap WITH (NOLOCK)
				INNER JOIN dbo.PARecord par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
			WHERE par.PeriodId = p.PeriodId AND pap.ProcessId = 4
				AND pap.EvaluatorId = @EmployeeId
		)

	INSERT INTO #announcement
	SELECT CONCAT(
		'Nominator duration for '
		,(CASE e.[Quarter]
			WHEN 1 THEN 'Employee of the Quarter - I'
			WHEN 2 THEN 'Employee of the Quarter - II'
			WHEN 3 THEN 'Employee of the Quarter - III'
			WHEN 4 THEN 'Employee of the Quarter - IV'
			ELSE CONCAT('Employee of the Year - ', @Year)
		END)
		,' will be from '
		,FORMAT(e.StartDateForNominator, @dateFormat)
		,' to '
		,FORMAT(e.EndDateForNominator, @dateFormat)
		,'.'
	)
	FROM dbo.EOQY e WITH (NOLOCK)
	WHERE @today BETWEEN e.StartDateForNominator AND e.EndDateForNominator
		AND EXISTS (
			SELECT 1
			FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
			WHERE e.EOQYId = ens.EOQYId AND ens.NominatorId = @EmployeeId 
			AND EXISTS (
				SELECT 1
				FROM dbo.func_SplitString(ens.RoleIds, ',') r
				WHERE r.val = 1
			)
		)

	INSERT INTO #announcement
	SELECT CONCAT(
		'Comittee duration for '
		,(CASE e.[Quarter]
			WHEN 1 THEN 'Employee of the Quarter - I'
			WHEN 2 THEN 'Employee of the Quarter - II'
			WHEN 3 THEN 'Employee of the Quarter - III'
			WHEN 4 THEN 'Employee of the Quarter - IV'
			ELSE CONCAT('Employee of the Year - ', @Year)
		END)
		,' will be from '
		,FORMAT(e.StartDateForNominator, @dateFormat)
		,' to '
		,FORMAT(e.EndDateForNominator, @dateFormat)
		,'.'
	)
	FROM dbo.EOQY e WITH (NOLOCK)
	WHERE @today > e.EndDateForNominator AND @today <= e.EndDateForCommittee
		AND EXISTS (
			SELECT 1
			FROM dbo.EOQYNominatorSetting ens WITH (NOLOCK)
			WHERE e.EOQYId = ens.EOQYId AND ens.NominatorId = @EmployeeId
			AND EXISTS (
				SELECT 1
				FROM dbo.func_SplitString(ens.RoleIds, ',') r
				WHERE r.val = 2
			)
		)


	SELECT [Message]
	FROM #announcement

	DROP TABLE #announcement
END