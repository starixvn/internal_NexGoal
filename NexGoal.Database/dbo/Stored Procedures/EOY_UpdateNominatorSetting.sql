
CREATE PROCEDURE [dbo].[EOY_UpdateNominatorSetting] @EOYId INT
	,@EmployeeId INT
	,@RoleIds NVARCHAR(10) = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180719@Tim.Trinh
Task: Update nominator setting.
DB: bodb_NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	IF ISNULL(@RoleIds, '') = ''
	BEGIN
		DELETE setting
		FROM dbo.EOQYNominatorSetting AS setting
		WHERE setting.EOQYId = @EOYId
			AND setting.NominatorId = @EmployeeId

		SET @xml = (
				SELECT EOQYId AS '@id'
					,NominatorId AS '@nid'
					,RoleIds AS '@r'
				FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
				WHERE EOQYId = @EOYId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	END
	ELSE
	BEGIN
		IF NOT EXISTS (
				SELECT 1
				FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
				WHERE EOQYId = @EOYId
					AND NominatorId = @EmployeeId
				)
		BEGIN
			INSERT INTO dbo.EOQYNominatorSetting (
				EOQYId
				,NominatorId
				,RoleIds
				)
			VALUES (
				@EOYId
				,@EmployeeId
				,@RoleIds
				)

			SET @xml = (
					SELECT EOQYId AS '@id'
						,NominatorId AS '@nid'
						,RoleIds AS '@r'
					FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
					WHERE EOQYId = @EOYId
					FOR XML PATH('r')
						,TYPE
					)
			SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		END
		ELSE
		BEGIN
			SET @xml = (
					SELECT EOQYId AS '@id'
						,NominatorId AS '@nid'
						,RoleIds AS '@r'
					FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
					WHERE EOQYId = @EOYId
					FOR XML PATH('r')
						,TYPE
					)
			SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

			UPDATE dbo.EOQYNominatorSetting WITH (ROWLOCK, UPDLOCK)
			SET RoleIds = @RoleIds
			WHERE EOQYId = @EOYId
				AND NominatorId = @EmployeeId

			SET @xml = (
					SELECT EOQYId AS '@id'
						,NominatorId AS '@nid'
						,RoleIds AS '@r'
					FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
					WHERE EOQYId = @EOYId
					FOR XML PATH('r')
						,TYPE
					)
			SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		END
	END
	
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'EOY_UpdateNominatorSetting'
		,@ObjectId = @EOYId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END
