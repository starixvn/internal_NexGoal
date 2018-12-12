CREATE PROCEDURE [InsertEOQTests].[test copy latest setting]
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
		,NominatorId INT
		,RoleIds VARCHAR(10)
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.Employee'
	
	EXEC tSQLt.FakeTable @TableName = 'dbo.EOQY'
		,@Identity = 1
	
	EXEC tSQLt.FakeTable @TableName = 'dbo.EOQYNominatorSetting'

	-- Act
	INSERT INTO dbo.Employee (EmployeeId, [Enabled], IsDeleted)
	VALUES (1, 1, 0)
		,(2, 1, 0)
		,(3, 0, 0)
	
	INSERT INTO dbo.EOQY([Quarter], [Year], IsQuarter, [Description], StartDateForNominator, EndDateForNominator, RemindDateForNominator, EndDateForCommittee)
	VALUES (1, 2018, 1, 'test', '2018-11-01', '2018-11-07', '2018-11-04', '2018-12-07')
	
	INSERT INTO dbo.EOQYNominatorSetting(EOQYId, NominatorId, RoleIds)
	VALUES (1, 1, '1')
		,(1, 2, '1')
		,(1, 3, '1,2')
	
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
	VALUES (2, 1, '1')
		,(2, 2, '1')

	SELECT EOQYId
		,NominatorId
		,RoleIds
	INTO [InsertEOQTests].[actual]
	FROM dbo.EOQYNominatorSetting
	WHERE EOQYId = 2

	EXEC tSQLt.AssertEqualsTable '[InsertEOQTests].[expected]'
		,'[InsertEOQTests].[actual]'
END
