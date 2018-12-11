CREATE PROCEDURE [GetEOQTests].[test returns all quarter periods]
AS
BEGIN
	IF OBJECT_ID('[GetEOQTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [GetEOQTests].[actual]
	END

	IF OBJECT_ID('GetEOQTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [GetEOQTests].[expected]
	END

	CREATE TABLE [GetEOQTests].[expected] (
		EOQYId INT
		,[Quarter] TINYINT
		,[Year] INT
		,StartDateForNominator DATE
		,EndDateForNominator DATE
		,RemindDateForNominator DATE
		,EndDateForCommittee DATE
		,[Description] NVARCHAR(200)
		)
	
	CREATE TABLE [GetEOQTests].[actual] (
		EOQYId INT
		,[Quarter] TINYINT
		,[Year] INT
		,StartDateForNominator DATE
		,EndDateForNominator DATE
		,RemindDateForNominator DATE
		,EndDateForCommittee DATE
		,[Description] NVARCHAR(200)
		)

	INSERT INTO [GetEOQTests].[expected]
	VALUES (1, 1, 2018, '2018-11-01', '2018-11-07', '2018-11-04', '2018-12-07', 'test')

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.EOQY'
		,@Identity = 1

	-- Act
	INSERT INTO dbo.EOQY([Quarter], [Year], IsQuarter, [Description], StartDateForNominator, EndDateForNominator, RemindDateForNominator, EndDateForCommittee)
	VALUES (1, 2018, 1, 'test', '2018-11-01', '2018-11-07', '2018-11-04', '2018-12-07')
		,(NULL, 2018, 0, 'test', '2018-12-01', '2018-12-07', '2018-12-04', '2018-12-14')

	INSERT INTO [GetEOQTests].[actual]
	EXEC dbo.EOQ_Get

	-- Assert
	EXEC tSQLt.AssertEqualsTable '[GetEOQTests].[expected]'
		,'[GetEOQTests].[actual]'
END
