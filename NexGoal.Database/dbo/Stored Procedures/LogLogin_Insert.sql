

CREATE PROCEDURE [dbo].[LogLogin_Insert]
	@IdentifyUser VARCHAR(50)
	,@LoginUser VARCHAR(50)
	,@MachineName VARCHAR(100) = ''
	,@IPAddress VARCHAR(50)
	,@BrowserName VARCHAR(50) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20161013@Quinn.Vo
Task: Insert log login.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Roles VARCHAR(20)
	SELECT @Roles = ''

	SELECT @Roles = @Roles + CAST(er.RoleId AS VARCHAR(1)) + ','
	FROM [dbo].[EmployeeRole] AS er WITH (NOLOCK)
		INNER JOIN [dbo].[Employee] AS e WITH (NOLOCK) ON er.EmployeeId = e.EmployeeId
	WHERE e.DomainUserName = 'spvn\' + @LoginUser

	IF(LEN(@Roles) > 0)
	BEGIN
		SELECT @Roles = LEFT(@Roles,LEN(@Roles) -1)
	END

	INSERT INTO dbo.LogLogin (
		IdentifyUser
		,LoginUser
		,Roles
		,MachineName
		,IPAddress
		,BrowserName
		,LoginDate
		)
	VALUES (
		@IdentifyUser
		,@LoginUser
		,@Roles
		,@MachineName
		,@IPAddress
		,@BrowserName
		,GETDATE()
		)
END