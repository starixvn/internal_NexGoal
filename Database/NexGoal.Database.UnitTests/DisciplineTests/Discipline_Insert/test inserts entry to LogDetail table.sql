
CREATE PROCEDURE [InsertDisciplineTests].[test inserts entry to LogDetail table]
AS
BEGIN
	IF OBJECT_ID('[InsertDisciplineTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [InsertDisciplineTests].[actual]
	END

	IF OBJECT_ID('InsertDisciplineTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [InsertDisciplineTests].[expected]
	END

	CREATE TABLE [InsertDisciplineTests].[expected] (
		LogId INT
		,ActionId INT
		,ActorId INT
		,ObjectId INT
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.Discipline'
		,@Identity = 1

	EXEC tSQLt.FakeTable @TableName = 'dbo.LogAction'
		,@Identity = 1

	EXEC tSQLt.FakeTable @TableName = 'dbo.LogDetail'
		,@Identity = 1

	-- Act
	INSERT INTO dbo.LogAction([Name], ActionTypeId, SPName, Items, SubItems)
	VALUES ('Insert Discipline', 3, 'Discipline_Insert', '', '')

	EXEC dbo.Discipline_Insert @EmployeeId = 1
		,@EffectiveDate = '2018-12-07'
		,@Description = 'test'
		,@CreatedBy = 1

	-- Assert
	SELECT LogId
		,ActionId
		,ActorId
		,ObjectId
	INTO [InsertDisciplineTests].[actual]
	FROM dbo.LogDetail

	INSERT INTO [InsertDisciplineTests].[expected]
	VALUES (1, 1, 1, 1)

	EXEC tSQLt.AssertEqualsTable '[InsertDisciplineTests].[expected]'
		,'[InsertDisciplineTests].[actual]'
END