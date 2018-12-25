
CREATE PROCEDURE [dbo].[NominationPeriod_UpdateSetting] @NominationPeriodId INT
	,@EmployeeId INT
	,@RoleIds NVARCHAR(10) = NULL
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181122@Tim.Trinh
Task: Update setting.
DB: bodb_NexGoal
Original: 

Revisions:  
Param's Explanation:
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

	IF ISNULL(@RoleIds, '') = ''
	BEGIN
		DELETE setting
		FROM dbo.EOQYNominatorSetting AS setting
		WHERE setting.EOQYId = @NominationPeriodId
			AND setting.NominatorId = @EmployeeId

		SET @xml = (
				SELECT EOQYId AS '@id'
					,NominatorId AS '@nid'
					,RoleIds AS '@r'
				FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
				WHERE EOQYId = @NominationPeriodId
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
				WHERE EOQYId = @NominationPeriodId
					AND NominatorId = @EmployeeId
				)
		BEGIN
			INSERT INTO dbo.EOQYNominatorSetting (
				EOQYId
				,NominatorId
				,RoleIds
				)
			VALUES (
				@NominationPeriodId
				,@EmployeeId
				,@RoleIds
				)

			SET @xml = (
					SELECT EOQYId AS '@id'
						,NominatorId AS '@nid'
						,RoleIds AS '@r'
					FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
					WHERE EOQYId = @NominationPeriodId
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
					WHERE EOQYId = @NominationPeriodId
					FOR XML PATH('r')
						,TYPE
					)
			SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

			UPDATE dbo.EOQYNominatorSetting WITH (ROWLOCK, UPDLOCK)
			SET RoleIds = @RoleIds
			WHERE EOQYId = @NominationPeriodId
				AND NominatorId = @EmployeeId

			SET @xml = (
					SELECT EOQYId AS '@id'
						,NominatorId AS '@nid'
						,RoleIds AS '@r'
					FROM dbo.EOQYNominatorSetting WITH (NOLOCK)
					WHERE EOQYId = @NominationPeriodId
					FOR XML PATH('r')
						,TYPE
					)
			SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		END
	END
	
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'NominationPeriod_UpdateSetting'
		,@ObjectId = @NominationPeriodId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END