
CREATE PROCEDURE [dbo].[PAFocus_Generate] @PeriodId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181008@Tim.Trinh
Task: PA Focus
DB: bobd_NexGoal
Original:

Revisions: 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @employeeId INT
		,@lastObjectiveId INT
		,@nextObjectiveId INT
		,@rowIndex INT = 1
		,@totalRow INT
		-- PA Status
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3

	CREATE TABLE #tblEmployeeIds (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,EmployeeId INT
		)

	INSERT INTO #tblEmployeeIds (EmployeeId)
	SELECT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS record WITH (NOLOCK) ON emp.EmployeeId = record.EmployeeId
		AND record.StatusId IN (@PAOpened, @PAInProgress)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON record.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		AND period.IsActive = 1
		LEFT JOIN dbo.PAFocus AS focus WITH (NOLOCK) ON emp.EmployeeId = focus.EmployeeId
		AND focus.PeriodId = @PeriodId
	WHERE emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		AND focus.EmployeeId IS NULL

	SELECT @totalRow = COUNT(1)
	FROM #tblEmployeeIds

	WHILE @rowIndex <= @totalRow
	BEGIN
		SELECT @employeeId = EmployeeId
		FROM #tblEmployeeIds
		WHERE RowNum = @rowIndex

		SELECT TOP 1 @lastObjectiveId = ObjectiveId
		FROM dbo.EmployeeObjective WITH (NOLOCK)
		WHERE EmployeeId = @employeeId
		ORDER BY CreatedDate DESC

		INSERT INTO dbo.EmployeeObjective
		VALUES (
			@employeeId
			,NULL
			,NULL
			,GETDATE()
			)

		SET @nextObjectiveId = SCOPE_IDENTITY()

		INSERT INTO dbo.PAFocus (
			PeriodId
			,EmployeeId
			,LastObjectiveId
			,NextObjectiveId
			,IsReadyPromotion
			,Contribution
			,BonusGiven
			,ModifiedBy
			,ModifiedDate
			)
		SELECT @PeriodId
			,@employeeId
			,@lastObjectiveId
			,@nextObjectiveId
			,NULL
			,NULL
			,NULL
			,NULL
			,NULL

		SET @rowIndex = @rowIndex + 1
	END

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to generate PA Focus.'

		RETURN
	END

	DROP TABLE #tblEmployeeIds
END