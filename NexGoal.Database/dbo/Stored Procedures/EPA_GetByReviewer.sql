


CREATE PROCEDURE [dbo].[EPA_GetByReviewer] @ReviewerId INT
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@PeriodIds VARCHAR(MAX) = NULL
AS
/*
ALTERd: 20150824@Tim.Trinh
Task: Get EPA by Reviewer.
DB: SMSEPA
Original:

Revisions:  
    20160229@Razor.Nguyen: add Monthly
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds INTID
		,@Periods INTID
		,@Employees INTID
		,@ToDay DATE
		,@Groups INTID
		,@EvaluatorProcessId INT;

	SET @ToDay = CAST(GETDATE() AS DATE);
	SET @EvaluatorProcessId = 3;

	IF ISNULL(@EmployeeIds, '') <> ''
	BEGIN
		INSERT INTO @Employees (Id)
		SELECT DISTINCT val
		FROM fnc_strSplit(@EmployeeIds, ',');
	END;

	IF ISNULL(@PeriodIds, '') <> ''
	BEGIN
		INSERT INTO @Periods (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@PeriodIds, ',');
	END;

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO @Groups (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@GroupIds, ',');
	END;

	INSERT INTO @EPAIds
	SELECT DISTINCT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON epa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON epa.EMployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON epa.EPAId = pc.EPAId
		AND pc.ProcessId = 4 -- reviewer
		AND pc.EvaluatorId = @ReviewerId
		AND pc.StartDate <= GETDATE()
	WHERE (
			@EmployeeIds IS NULL
			OR epa.EmployeeId IN (
				SELECT Id
				FROM @Employees
				)
			)
		AND (
			@GroupIds IS NULL
			OR gr.GroupId IN (
				SELECT Id
				FROM @Groups
				)
			)
		AND (
			@PeriodIds IS NULL
			OR period.PeriodId IN (
				SELECT Id
				FROM @Periods
				)
			);

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds;

	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds;
END;
