
CREATE PROCEDURE [dbo].[Auto_PAFocus_Generate] @PeriodIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181220@Tim.Trinh
Task: generate PA Focus.
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

	WHILE @index <= @total
	BEGIN
		SELECT @periodId = PeriodId
		FROM #tblPeriodIds WITH (NOLOCK)
		WHERE RowNum = @index

		EXEC dbo.PAFocus_Generate @PeriodId = @periodId
			,@ErrCode = @ErrCode OUT
			,@ErrMsg = @ErrMsg OUT

		SET @index = @index + 1
	END

	DROP TABLE #tblPeriodIds
END