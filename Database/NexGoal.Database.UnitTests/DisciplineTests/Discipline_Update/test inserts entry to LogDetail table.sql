
CREATE PROCEDURE [UpdateDisciplineTests].[test inserts entry to LogDetail table]
AS
BEGIN
	IF OBJECT_ID('[UpdateDisciplineTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [UpdateDisciplineTests].[actual]
	END

	IF OBJECT_ID('UpdateDisciplineTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [UpdateDisciplineTests].[expected]
	END

	CREATE TABLE [UpdateDisciplineTests].[expected] (
		LogId INT
		,ActionId INT
		,ActorId INT
		,ObjectId INT
		)

	INSERT INTO [UpdateDisciplineTests].[expected]
	VALUES (
		1
		,1
		,2
		,1
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.Discipline'
		,@Identity = 1

	EXEC tSQLt.FakeTable @TableName = 'dbo.LogAction'
		,@Identity = 1

	EXEC tSQLt.FakeTable @TableName = 'dbo.LogDetail'
		,@Identity = 1

	-- Act
	INSERT INTO dbo.LogAction
	VALUES (
		'Update Discipline'
		,3
		,'Discipline_Update'
		,''
		,''
		)

	EXEC dbo.Discipline_Update @DisciplineId = 1
		,@EffectiveDate = '2018-12-10'
		,@Description = 'update'
		,@ModifiedBy = 2

	-- Assert
	SELECT LogId
		,ActionId
		,ActorId
		,ObjectId
	INTO [UpdateDisciplineTests].[actual]
	FROM dbo.LogDetail

	EXEC tSQLt.AssertEqualsTable '[UpdateDisciplineTests].[expected]'
		,'[UpdateDisciplineTests].[actual]'
END