CREATE PROCEDURE [InsertEOQTests].[test inserts a new quarter period]
AS
BEGIN
	IF OBJECT_ID('[InsertEOQTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [InsertEOQTests].[actual]
	END

	IF OBJECT_ID('InsertEOQTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [InsertEOQTests].[expected]
	END

	CREATE TABLE [InsertEOQTests].[expected] (
		EOQYId INT
		,[Quarter] TINYINT
		,[Year] INT
		,StartDateForNominator DATE
		,EndDateForNominator DATE
		,RemindDateForNominator DATE
		,EndDateForCommittee DATE
		,[Description] NVARCHAR(200)
		,IsQuarter BIT
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.EOQY'
		,@Identity = 1

	-- Act
	INSERT INTO [InsertEOQTests].[expected]
	VALUES (1, 1, 2018, '2018-11-01', '2018-11-07', '2018-11-04', '2018-12-07', 'test', 1)
	
	EXEC dbo.EOQ_Insert @Quarter = 1
		,@Year = 2018
		,@StartDateForNominator = '2018-11-01'
		,@EndDateForNominator = '2018-11-07'
		,@RemindDateForNominator = '2018-11-04'
		,@EndDateForCommittee = '2018-12-07'
		,@Description = 'test'
		,@CreatedBy = 1

	-- Assert
	SELECT EOQYId
		,[Quarter]
		,[Year]
		,StartDateForNominator
		,EndDateForNominator
		,RemindDateForNominator
		,EndDateForCommittee
		,[Description]
		,IsQuarter
	INTO [InsertEOQTests].[actual]
	FROM dbo.EOQY

	EXEC tSQLt.AssertEqualsTable '[InsertEOQTests].[expected]'
		,'[InsertEOQTests].[actual]'
END
