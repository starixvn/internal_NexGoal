


CREATE PROCEDURE [dbo].[EPA_GetByEmployee] @EmployeeId INT
	,@PeriodIds VARCHAR(MAX) = NULL
AS
/*
ALTERd: 20150710@Tim.Trinh
Task: Get EPAs by employeeId.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EPAIds INTID
		,@Periods INTID
		,@ToDay DATE
		,@EmployeeProcessId INT;

	SET @EmployeeProcessId = 2;
	SET @ToDay = CAST(GETDATE() AS DATE);

	IF ISNULL(@PeriodIds, '') <> ''
	BEGIN
		INSERT INTO @Periods (Id)
		SELECT DISTINCT fnc_strSplit.val
		FROM dbo.fnc_strSplit(@PeriodIds, ',');
	END;

	INSERT INTO @EPAIds (Id)
	SELECT epa.EPAId
	FROM dbo.EPA AS epa WITH (NOLOCK)
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON p.PeriodId = epa.PeriodId
		INNER JOIN dbo.EPAProcess AS pc WITH (NOLOCK) ON pc.epaId = epa.EPAId
	WHERE epa.EmployeeId = @EmployeeId
		AND p.IsActive = 1
		AND epa.[Status] <> 2
		AND pc.StartDate <= GETDATE()
		AND pc.ProcessId = @EmployeeProcessId
		AND (
			@PeriodIds IS NULL
			OR p.PeriodId IN (
				SELECT Id
				FROM @Periods
				)
			);

	-- General Info
	EXECUTE dbo.EPA_GetGeneralInfo @EPAIds;
	
	-- Process
	EXECUTE dbo.EPA_GetProcess @EPAIds;
END;
