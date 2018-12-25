
CREATE PROCEDURE [dbo].[Employee_UpdateRoles] @EmployeeId INT
	,@ModifiedBy INT
	,@SelectedRoleIds VARCHAR(255) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180705@Elton.Ton
Task: update employee roles
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	DECLARE @SelectedRole TABLE (RoleId INT)

	INSERT INTO @SelectedRole
	SELECT CAST(val AS INT)
	FROM dbo.func_SplitString(@SelectedRoleIds, ',')
	
	-- Write log before assign permission	
	SET @xml = (
			SELECT er.RoleId AS '@ri'
				,er.EmployeeId AS '@ei'
			FROM dbo.EmployeeRole er WITH (NOLOCK)
			WHERE er.EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE er
	FROM dbo.EmployeeRole AS er
	WHERE er.EmployeeId = @EmployeeId
		AND NOT EXISTS (
			SELECT 1
			FROM @SelectedRole sr
			WHERE er.RoleId = sr.RoleId
			)

	INSERT INTO dbo.EmployeeRole (
		EmployeeId
		,RoleId
		)
	SELECT @EmployeeId
		,sr.RoleId
	FROM @SelectedRole AS sr
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole AS er WITH (NOLOCK)
			WHERE er.EmployeeId = @EmployeeId
				AND er.RoleId = sr.RoleId
			)

	-- write log after update
	SET @xml = (
			SELECT er.RoleId AS '@ri'
				,er.EmployeeId AS '@ei'
			FROM dbo.EmployeeRole er WITH (NOLOCK)
			WHERE er.EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Employee_UpdateRoles'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END
