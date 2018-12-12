CREATE PROCEDURE [InsertEOQTests].[test inserts entry to LogDetail table]
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
		LogId INT
		,ActionId INT
		,ActorId INT
		,ObjectId INT
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.EOQY'
		,@Identity = 1
	
	EXEC tSQLt.FakeTable @TableName = 'dbo.LogAction'
		,@Identity = 1

	EXEC tSQLt.FakeTable @TableName = 'dbo.LogDetail'
		,@Identity = 1

	-- Act
	INSERT INTO dbo.LogAction([Name], ActionTypeId, SPName, Items, SubItems)
	VALUES ('Insert EOQ', 4, 'EOQ_Insert', '', '')
	
	EXEC dbo.EOQ_Insert @Quarter = 1
		,@Year = 2019
		,@StartDateForNominator = '2019-11-01'
		,@EndDateForNominator = '2019-11-07'
		,@RemindDateForNominator = '2019-11-04'
		,@EndDateForCommittee = '2019-12-07'
		,@Description = 'test'
		,@CreatedBy = 1

	-- Assert
	INSERT INTO [InsertEOQTests].[expected]
	VALUES (1, 1, 1, 1)
	
	SELECT LogId
		,ActionId
		,ActorId
		,ObjectId
	INTO [InsertEOQTests].[actual]
	FROM dbo.LogDetail

	EXEC tSQLt.AssertEqualsTable '[InsertEOQTests].[expected]'
		,'[InsertEOQTests].[actual]'
END
