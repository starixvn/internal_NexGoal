
CREATE PROCEDURE [UpdateStatusDisciplineTests].[test updates status an existing discipline]
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
		DisciplineId INT
		,EmployeeId INT
		,[Description] NVARCHAR(MAX)
		,IsActive BIT
		,EffectiveDate DATE
		,CreatedBy INT
		,CreatedDate DATETIME
		,ModifiedBy INT
		)

	INSERT INTO [UpdateStatusDisciplineTests].[expected]
	VALUES (
		1
		,1
		,'insert'
		,0
		,'2018-12-07'
		,1
		,'2018-12-07'
		,2
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.Discipline'
		,@Identity = 1

	-- Act
	INSERT INTO dbo.Discipline
	VALUES (
		1
		,'insert'
		,1
		,'2018-12-07'
		,1
		,'2018-12-07'
		,1
		,NULL
		)

	EXEC dbo.Discipline_UpdateStatus @DisciplineId = 1
		,@IsActive = 0
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
	INTO [UpdateStatusDisciplineTests].[actual]
	FROM dbo.Discipline

	EXEC tSQLt.AssertEqualsTable '[UpdateStatusDisciplineTests].[expected]'
		,'[UpdateStatusDisciplineTests].[actual]'
END