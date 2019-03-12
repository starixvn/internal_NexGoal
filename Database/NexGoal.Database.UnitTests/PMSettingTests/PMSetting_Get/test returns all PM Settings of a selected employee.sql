CREATE PROCEDURE GetPMSettingsTests.[test returns all PM Settings of a selected employee]
AS
BEGIN
	IF OBJECT_ID('[GetPMSettingsTests].[actual]') IS NOT NULL
	BEGIN
		DROP TABLE [GetPMSettingsTests].[actual]
	END

	IF OBJECT_ID('GetPMSettingsTests].expected') IS NOT NULL
	BEGIN
		DROP TABLE [GetPMSettingsTests].[expected]
	END

	CREATE TABLE [GetPMSettingsTests].[expected] (
		EmployeeId INT
		,Code VARCHAR(10)
		,FullName NVARCHAR(50)
		,NickName NVARCHAR(50)
		,JobTitle VARCHAR(50)
		,ProjectManagerId INT
	)
	
	CREATE TABLE [GetPMSettingsTests].[actual] (
		EmployeeId INT
		,Code VARCHAR(10)
		,FullName NVARCHAR(50)
		,NickName NVARCHAR(50)
		,JobTitle VARCHAR(50)
		,ProjectManagerId INT
	)

	-- Fake table
	EXEC tSQLt.FakeTable @TableName = 'dbo.JobTitle'

	EXEC tSQLt.FakeTable @TableName = 'dbo.Employee'

	EXEC tSQLt.FakeTable @TableName = 'dbo.PMSetting'

	INSERT INTO dbo.JobTitle(JobTitleId, [Name])
	VALUES (1, 'Project Manager')
		,(2, 'Software Enginner')
	
	INSERT INTO dbo.Employee(EmployeeId, FullName, NickName, JobTitleId, [Enabled], IsDeleted)
	VALUES (1, 'Phu', 'Michael', 1, 1, 0)
		,(2, 'Long', 'Elton', 2, 1, 0)
		,(3, 'Khanh', 'Khanh', 2, 1, 0)
		,(4, 'HA', 'HA', 1, 1, 0)
	
	INSERT INTO dbo.PMSetting(MemberId, ProjectManagerId)
	VALUES (2, 1)
		,(3,1)
		,(2,4)

	INSERT INTO [GetPMSettingsTests].[expected]
	VALUES (2, null, 'Long', 'Elton', 'Software Enginner', 1)
	,(2, null, 'Long', 'Elton', 'Software Enginner', 4)


	INSERT INTO [GetPMSettingsTests].[actual]
	EXEC dbo.PMSetting_Get '1,4', 2

	-- Assert
	EXEC tSQLt.AssertEqualsTable '[GetPMSettingsTests].[expected]'
		,'[GetPMSettingsTests].[actual]'
END