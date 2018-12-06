--EXEC tSQLt.NewTestClass @ClassName = N'DisciplineTests'
CREATE PROCEDURE DisciplineTests.[test if Discipline table exsits]
AS
BEGIN
	EXEC tSQLt.AssertObjectExists @ObjectName = N'dbo.Discipline'
END