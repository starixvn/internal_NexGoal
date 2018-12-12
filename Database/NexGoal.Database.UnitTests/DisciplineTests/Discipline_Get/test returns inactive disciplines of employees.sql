CREATE PROCEDURE [GetDisciplineTests].[test returns inactive disciplines of employees]
AS
BEGIN
	IF OBJECT_ID('[GetDisciplineTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [GetDisciplineTests].[actual]
	END

	IF OBJECT_ID('GetDisciplineTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [GetDisciplineTests].[expected]
	END

	CREATE TABLE [GetDisciplineTests].[expected] (
		DisciplineId INT
		,[Description] NVARCHAR(MAX)
		,IsActive BIT
		,EffectiveDate DATE
		,EmployeeId INT
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(50)
		,GroupName NVARCHAR(50)
		,CreatedBy INT
		,CreatorNickName NVARCHAR(20)
		,CreatorFullName NVARCHAR(50)
		)
	
	CREATE TABLE [GetDisciplineTests].[actual] (
		DisciplineId INT
		,[Description] NVARCHAR(MAX)
		,IsActive BIT
		,EffectiveDate DATE
		,EmployeeId INT
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(50)
		,GroupName NVARCHAR(50)
		,CreatedBy INT
		,CreatorNickName NVARCHAR(20)
		,CreatorFullName NVARCHAR(50)
		)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.Discipline'
		,@Identity = 1

	EXEC tSQLt.FakeTable @TableName = 'dbo.Employee'

	EXEC tSQLt.FakeTable @TableName = 'dbo.Group'

	-- Act
	INSERT INTO dbo.[Group](GroupId, [Name], Code)
	VALUES (1, 'nexus', 'nex')
		,(2, 'web team leader', 'wtl')
	
	INSERT INTO dbo.Employee(EmployeeId, Code, FullName, NickName, GroupId, [Enabled], IsDeleted)
	VALUES (1, '00329', 'trinh dang thang', 'tim', 1, 1, 0)
		,(2, '00320', 'vo kim quang', 'quinn', 2, 1, 0)
	
	INSERT INTO dbo.Discipline(EmployeeId, [Description], IsActive, EffectiveDate, CreatedBy, CreatedDate)
	VALUES (1, 'test', 0, '2018-12-07', 2, '2018-12-07')
		,(2, 'test', 1, '2018-12-07', 2, '2018-12-07')

	INSERT INTO [GetDisciplineTests].[actual]
	EXEC dbo.Discipline_Get @FromDate = '2018-10-01'
		,@ToDate = '2018-12-31'
		,@IsActive = 0

	-- Assert
	INSERT INTO [GetDisciplineTests].[expected]
	VALUES (1, 'test', 0, '2018-12-07', 1, 'tim', 'trinh dang thang', 'nexus', 2, 'quinn', 'vo kim quang')

	EXEC tSQLt.AssertEqualsTable '[GetDisciplineTests].[expected]'
		,'[GetDisciplineTests].[actual]'
END