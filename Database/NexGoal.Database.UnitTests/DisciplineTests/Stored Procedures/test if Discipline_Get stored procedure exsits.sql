--EXEC tSQLt.NewTestClass @ClassName = N'DisciplineTests'
CREATE PROCEDURE DisciplineTests.[test if Discipline_Get stored procedure exsits]
AS
BEGIN
	EXEC tSQLt.AssertObjectExists @ObjectName = N'dbo.Discipline_Get'
END