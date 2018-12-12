
CREATE PROCEDURE [UpdateStatusDisciplineTests].[test inserts entry to LogDetail table]
AS
BEGIN
	IF OBJECT_ID('[UpdateStatusDisciplineTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [UpdateStatusDisciplineTests].[actual]
	END

	IF OBJECT_ID('UpdateStatusDisciplineTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [UpdateStatusDisciplineTests].[expected]
	END

	CREATE TABLE [UpdateStatusDisciplineTests].[expected] (
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
	VALUES ('Update Status Discipline', 3, 'Discipline_UpdateStatus', '', '')

	EXEC dbo.Discipline_UpdateStatus @DisciplineId = 1
		,@IsActive = 0
		,@ModifiedBy = 2

	-- Assert
	SELECT LogId
		,ActionId
		,ActorId
		,ObjectId
	INTO [UpdateStatusDisciplineTests].[actual]
	FROM dbo.LogDetail

	INSERT INTO [UpdateStatusDisciplineTests].[expected]
	VALUES (1, 1, 2, 1)

	EXEC tSQLt.AssertEqualsTable '[UpdateStatusDisciplineTests].[expected]'
		,'[UpdateStatusDisciplineTests].[actual]'
END