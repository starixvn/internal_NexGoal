
CREATE PROCEDURE [dbo].[Auto_Period_InsertEmployeeSettings] @PeriodIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180919@Tim.Trinh
Task: insert employee settings to period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @index INT = 1
		,@total INT
		,@groupIds VARCHAR(MAX)
		,@periodId INT

	CREATE TABLE #tblPeriodIds (
		RowNum INT IDENTITY(1, 1)
		,PeriodId INT NOT NULL
		)

	INSERT INTO #tblPeriodIds (PeriodId)
	SELECT val
	FROM dbo.func_SplitString(@PeriodIds, ',')
	WHERE ISNUMERIC(val) = 1

	SELECT @total = COUNT(1)
	FROM #tblPeriodIds WITH (NOLOCK)

	SELECT @groupIds = COALESCE(@groupIds + ',', '') + CAST(GroupId AS VARCHAR(10))
	FROM dbo.[Group] WITH (NOLOCK)
	WHERE IsDeleted = 0

	WHILE @index <= @total
	BEGIN
		SELECT @periodId = PeriodId
		FROM #tblPeriodIds WITH (NOLOCK)
		WHERE RowNum = @index

		EXEC dbo.Period_InsertEmployeeSettings @PeriodId = @periodId
			,@GroupIds = @groupIds
			,@CreatedBy = - 1
			,@ErrCode = @ErrCode OUT
			,@ErrMsg = @ErrMsg OUT

		SET @index = @index + 1
	END

	DROP TABLE #tblPeriodIds
END
