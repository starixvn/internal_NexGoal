CREATE PROCEDURE [InsertDisciplineTests].[test inserts a new discipline]
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
		DisciplineId INT
		,EmployeeId INT
		,[Description] NVARCHAR(MAX)
		,IsActive BIT
		,EffectiveDate DATE
		,CreatedBy INT
		,ModifiedBy INT
		,ModifiedDate DATETIME
		)

	INSERT INTO [InsertDisciplineTests].[expected]
	VALUES (
		1
		,1
		,'test'
		,1
		,'2018-12-07'
		,1
		,NULL
		,NULL
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.Discipline'
		,@Identity = 1

	-- Act
	EXEC dbo.Discipline_Insert @EmployeeId = 1
		,@EffectiveDate = '2018-12-07'
		,@Description = 'test'
		,@CreatedBy = 1

	-- Assert
	SELECT DisciplineId
		,EmployeeId
		,[Description]
		,IsActive
		,EffectiveDate
		,CreatedBy
		,ModifiedBy
		,ModifiedDate
	INTO [InsertDisciplineTests].[actual]
	FROM dbo.Discipline

	EXEC tSQLt.AssertEqualsTable '[InsertDisciplineTests].[expected]'
		,'[InsertDisciplineTests].[actual]'
END