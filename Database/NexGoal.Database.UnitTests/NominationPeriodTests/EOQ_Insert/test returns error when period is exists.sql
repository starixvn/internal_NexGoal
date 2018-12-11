CREATE PROCEDURE [InsertEOQTests].[test returns error when period is exists]
AS
BEGIN
	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.EOQY'

	-- Act
	DECLARE @actualErrorCode INT
		,@actualErrorMessage NVARCHAR(200)

	INSERT INTO dbo.EOQY(EOQYId, [Quarter], [Year], IsQuarter, [Description], StartDateForNominator, EndDateForNominator, RemindDateForNominator, EndDateForCommittee)
	VALUES (1, 1, 2018, 1, 'test', '2018-11-01', '2018-11-07', '2018-11-04', '2018-12-07')
	
	EXEC dbo.EOQ_Insert @Quarter = 1
		,@Year = 2018
		,@StartDateForNominator = '2018-12-01'
		,@EndDateForNominator = '2018-12-07'
		,@RemindDateForNominator = '2018-12-05'
		,@EndDateForCommittee = '2018-12-20'
		,@Description = NULL
		,@CreatedBy = 1
		,@ErrCode = @actualErrorCode OUTPUT
		,@ErrMsg = @actualErrorMessage OUTPUT

	-- Assert
	DECLARE @expectedErrorCode INT = 1
		,@expectedErrorMessage NVARCHAR(200) = N'EOQ already exists.'

	EXEC tSQLt.AssertEquals @expectedErrorCode
		,@actualErrorCode

	EXEC tSQLt.AssertEqualsString @expectedErrorMessage
		,@actualErrorMessage
END
