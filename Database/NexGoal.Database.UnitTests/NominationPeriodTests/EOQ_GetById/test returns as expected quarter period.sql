CREATE PROCEDURE [GetEOQByIdTests].[test returns as expected quarter period]
AS
BEGIN
	IF OBJECT_ID('[GetEOQByIdTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [GetEOQByIdTests].[actual]
	END

	IF OBJECT_ID('GetEOQByIdTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [GetEOQByIdTests].[expected]
	END

	CREATE TABLE [GetEOQByIdTests].[expected] (
		EOQYId INT
		,[Quarter] TINYINT
		,[Year] INT
		,StartDateForNominator DATE
		,EndDateForNominator DATE
		,RemindDateForNominator DATE
		,EndDateForCommittee DATE
		,[Description] NVARCHAR(200)
		)
	
	CREATE TABLE [GetEOQByIdTests].[actual] (
		EOQYId INT
		,[Quarter] TINYINT
		,[Year] INT
		,StartDateForNominator DATE
		,EndDateForNominator DATE
		,RemindDateForNominator DATE
		,EndDateForCommittee DATE
		,[Description] NVARCHAR(200)
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.EOQY'

	-- Act
	INSERT INTO dbo.EOQY(EOQYId, [Quarter], [Year], IsQuarter, [Description], StartDateForNominator, EndDateForNominator, RemindDateForNominator, EndDateForCommittee)
	VALUES (1, 1, 2018, 1, 'test', '2018-11-01', '2018-11-07', '2018-11-04', '2018-12-07')
		,(2, 1, 2019, 1, 'test', '2019-12-01', '2019-12-07', '2019-12-04', '2019-12-14')

	INSERT INTO [GetEOQByIdTests].[actual]
	EXEC dbo.EOQ_GetById @EOQId = 1

	-- Assert
	INSERT INTO [GetEOQByIdTests].[expected]
	VALUES (1, 1, 2018, '2018-11-01', '2018-11-07', '2018-11-04', '2018-12-07', 'test')

	EXEC tSQLt.AssertEqualsTable '[GetEOQByIdTests].[expected]'
		,'[GetEOQByIdTests].[actual]'
END
