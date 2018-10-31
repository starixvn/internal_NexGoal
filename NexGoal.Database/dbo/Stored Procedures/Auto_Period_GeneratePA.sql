
CREATE PROCEDURE [dbo].[Auto_Period_GeneratePA] @PeriodIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180919@Tim.Trinh
Task: generate PA.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @index INT = 1
		,@total INT
		,@paRecordIds VARCHAR(MAX)
		,@periodId INT

	CREATE TABLE #tblPeriodIds (
		Id INT IDENTITY(1, 1)
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
		WHERE Id = @index

		SELECT @paRecordIds = COALESCE(@paRecordIds + ',', '') + CAST(PARecordId AS VARCHAR(10))
		FROM dbo.PARecord WITH (NOLOCK)
		WHERE PeriodId = @periodId
			AND StatusId = 1 -- not generated

		EXEC dbo.Period_GeneratePA @PeriodId = @periodId
			,@PARecordIds = @paRecordIds
			,@CreatedBy = - 1
			,@ErrCode = @ErrCode OUT
			,@ErrMsg = @ErrMsg OUT

		SET @index = @index + 1
	END

	DROP TABLE #tblPeriodIds
END
