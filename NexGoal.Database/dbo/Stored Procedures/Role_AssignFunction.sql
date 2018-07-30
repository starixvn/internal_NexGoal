
CREATE PROCEDURE [dbo].[Role_AssignFunction] @RoleId SMALLINT
	,@Value VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180628@Tim.Trinh
Task: Assign function to role
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
    Value : '<ROOT><Row FunctionId="4" Policy="1"></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@idoc INT

	-- Write log before assign permission	
	SET @xml = (
			SELECT @RoleId AS '@r'
				,(
					SELECT FunctionId AS '@f'
						,Policy AS '@p'
					FROM dbo.RoleFunction WITH (NOLOCK)
					WHERE RoleId = @RoleId
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	CREATE TABLE #tblFunctionPolicy (
		FunctionId SMALLINT
		,Policy TINYINT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Value

	INSERT INTO #tblFunctionPolicy (
		FunctionId
		,Policy
		)
	SELECT FunctionId
		,Policy
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (FunctionId SMALLINT, Policy TINYINT) AS data

	-- delete function not set to role
	DELETE rfunc
	FROM dbo.RoleFunction AS rfunc
		INNER JOIN #tblFunctionPolicy AS tbl WITH (NOLOCK) ON rfunc.FunctionId = tbl.FunctionId
	WHERE rfunc.RoleId = @RoleId
		AND tbl.Policy = 0

	-- update policy for function of role
	UPDATE rfunc
	SET rfunc.Policy = tbl.Policy
	FROM dbo.RoleFunction AS rfunc WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblFunctionPolicy AS tbl WITH (NOLOCK) ON rfunc.FunctionId = tbl.FunctionId
	WHERE rfunc.RoleId = @RoleId
		AND tbl.Policy > 0

	-- insert new policy for function of role
	INSERT INTO dbo.RoleFunction (
		RoleId
		,FunctionId
		,Policy
		)
	SELECT @RoleId
		,tbl.FunctionId
		,tbl.Policy
	FROM #tblFunctionPolicy AS tbl WITH (NOLOCK)
	WHERE tbl.Policy > 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.RoleFunction AS rfunc WITH (NOLOCK)
			WHERE rfunc.RoleId = @RoleId
				AND rfunc.FunctionId = tbl.FunctionId
			)

	-- write log after update
	SET @xml = (
			SELECT @RoleId AS '@r'
				,(
					SELECT FunctionId AS '@f'
						,Policy AS '@p'
					FROM dbo.RoleFunction WITH (NOLOCK)
					WHERE RoleId = @RoleId
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Role_AssignFunction'
		,@ObjectId = @RoleId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #tblFunctionPolicy
END