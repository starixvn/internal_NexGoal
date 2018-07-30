CREATE PROCEDURE [dbo].[User_Search] 
	@Keyword VARCHAR(250) = NULL
AS
/*
Created: 20180716@Elton.ton
Task: search users
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
		,e.FullName
		,e.NickName
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE @Keyword IS NULL OR e.FullName LIKE '%' + @Keyword + '%' OR e.NickName LIKE '%' + @Keyword + '%'
END