
CREATE PROCEDURE [UpdateDisciplineTests].[test updates an existing discipline]
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
		DisciplineId INT
		,EmployeeId INT
		,[Description] NVARCHAR(MAX)
		,IsActive BIT
		,EffectiveDate DATE
		,CreatedBy INT
		,CreatedDate DATETIME
		,ModifiedBy INT
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.Discipline'
		,@Identity = 1

	-- Act
	INSERT INTO dbo.Discipline
	VALUES (1, 'insert', 1, '2018-12-07', 1, '2018-12-07', NULL, NULL)

	EXEC dbo.Discipline_Update @DisciplineId = 1
		,@EffectiveDate = '2018-12-10'
		,@Description = 'update'
		,@ModifiedBy = 2

	-- Assert
	SELECT DisciplineId
		,EmployeeId
		,[Description]
		,IsActive
		,EffectiveDate
		,CreatedBy
		,CreatedDate
		,ModifiedBy
	INTO [UpdateDisciplineTests].[actual]
	FROM dbo.Discipline

	INSERT INTO [UpdateDisciplineTests].[expected]
	VALUES (1, 1, 'update', 1, '2018-12-10', 1, '2018-12-07', 2)

	EXEC tSQLt.AssertEqualsTable '[UpdateDisciplineTests].[expected]'
		,'[UpdateDisciplineTests].[actual]'
END