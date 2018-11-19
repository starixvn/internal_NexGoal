
CREATE PROCEDURE [dbo].[Nomination_Update] @NominationPeriodId INT
	,@NomineeId INT
	,@NominatorId INT
	,@Comment NVARCHAR(MAX) = NULL
	,@PerformanceRecordId INT
	,@PerformanceRecordStatusId TINYINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180924@Elton.Ton
Task: Nomiate an employee
DB: bodb_NexGoal
Original: 
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.NominationDetail AS nd WITH (NOLOCK)
				INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON n.NominationId = nd.NominationId
			WHERE nd.NominatorId = @NominatorId
				AND n.NomineeId = @NomineeId
				AND n.EOQYId = @NominationPeriodId
				AND nd.PerformanceRecordId <> @PerformanceRecordId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The employee has already been nominated in this period.'

		RETURN
	END

	IF @NominatorId = @NomineeId
		AND EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId = @NominationPeriodId
				AND IsQuarter = 1
				AND [Quarter] IS NOT NULL
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'You can''t nominate yourself.'

		RETURN
	END

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML
		,@NominationId INT
		,@CurrentNominationId INT
		,@CurrentNominationStatusId INT
		,@NumberOfNominations INT
		,@SubmittedStatusId TINYINT = 1
		,@DraftStatusId TINYINT = 4
		-- Performance Record Status
		,@PR_Draft TINYINT = 2
		,@PR_Submitted TINYINT = 3

	SET @xml = (
			SELECT n.NominationId AS '@ni'
				,n.EOQYId AS '@eoqyi'
				,n.NomineeId AS '@nei'
				,n.StatusId AS '@si'
				,nd.NominatorId AS '@nri'
				,nd.PerformanceRecordId AS '@pri'
				,pr.Comment AS '@prc'
				,pr.CreatedDate AS '@prcd'
				,pr.StatusId AS '@prsi'
				,pr.TypeId AS '@prti'
			FROM dbo.Nomination AS n WITH (NOLOCK)
				INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON n.NominationId = nd.NominationId
				INNER JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON nd.PerformanceRecordId = pr.PerformanceRecordId
			WHERE nd.NominatorId = @NominatorId
				AND nd.PerformanceRecordId = @PerformanceRecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.PerformanceRecord WITH (ROWLOCK, UPDLOCK)
	SET EmployeeIds = @NomineeId
		,Comment = @Comment
		,StatusId = @PerformanceRecordStatusId
		,ModifiedDate = GETDATE()
	WHERE PerformanceRecordId = @PerformanceRecordId

	SELECT @CurrentNominationId = nd.NominationId
		,@CurrentNominationStatusId = n.StatusId
	FROM dbo.NominationDetail AS nd WITH (NOLOCK)
		INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON nd.NominationId = n.NominationId
	WHERE nd.NominatorId = @NominatorId
		AND nd.PerformanceRecordId = @PerformanceRecordId

	SELECT @NominationId = n.NominationId
	FROM dbo.Nomination AS n WITH (NOLOCK)
	WHERE n.NomineeId = @NomineeId
		AND n.EOQYId = @NominationPeriodId

	DECLARE @NominationStatusId TINYINT = @SubmittedStatusId

	IF @PerformanceRecordStatusId = @PR_Draft
	BEGIN
		SET @NominationStatusId = @DraftStatusId
	END

	IF @CurrentNominationId = @NominationId
	BEGIN
		IF @CurrentNominationStatusId = @DraftStatusId
			AND @PerformanceRecordStatusId = @PR_Submitted
		BEGIN
			UPDATE dbo.Nomination WITH (ROWLOCK, UPDLOCK)
			SET StatusId = @NominationStatusId
			WHERE NominationId = @CurrentNominationId
		END
	END
	ELSE
	BEGIN
		SELECT @NumberOfNominations = COUNT(*)
		FROM dbo.NominationDetail AS nd WITH (NOLOCK)
		WHERE nd.NominationId = @CurrentNominationId

		IF @NumberOfNominations = 1
			AND @NominationId IS NULL
		BEGIN
			UPDATE dbo.Nomination WITH (ROWLOCK, UPDLOCK)
			SET NomineeId = @NomineeId
				,EOQYId = @NominationPeriodId
				,StatusId = @NominationStatusId
			WHERE NominationId = @CurrentNominationId
		END
		ELSE
		BEGIN
			IF @NominationId IS NULL
			BEGIN
				INSERT INTO [dbo].[Nomination] (
					[EOQYId]
					,[NomineeId]
					,[StatusId]
					)
				VALUES (
					@NominationPeriodId
					,@NomineeId
					,@NominationStatusId
					)

				SET @NominationId = SCOPE_IDENTITY()
			END

			UPDATE dbo.NominationDetail WITH (ROWLOCK, UPDLOCK)
			SET NominationId = @NominationId
			WHERE NominatorId = @NominatorId
				AND PerformanceRecordId = @PerformanceRecordId

			IF @NumberOfNominations = 1
			BEGIN
				DELETE dbo.Nomination
				WHERE NominationId = @CurrentNominationId
			END
			ELSE
			BEGIN
				IF @CurrentNominationStatusId = @SubmittedStatusId
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.NominationDetail nd WITH (NOLOCK)
							INNER JOIN dbo.PerformanceRecord pr ON nd.PerformanceRecordId = pr.PerformanceRecordId
						WHERE pr.PerformanceRecordId = @PerformanceRecordId
							AND nd.NominationId = @CurrentNominationId
						)
				BEGIN
					UPDATE dbo.Nomination WITH (ROWLOCK, UPDLOCK)
					SET StatusId = @DraftStatusId
					WHERE NominationId = @CurrentNominationId
				END
			END
		END
	END

	SET @xml = (
			SELECT n.NominationId AS '@ni'
				,n.EOQYId AS '@eoqyi'
				,n.NomineeId AS '@nei'
				,n.StatusId AS '@si'
				,nd.NominatorId AS '@nri'
				,nd.PerformanceRecordId AS '@pri'
				,pr.Comment AS '@prc'
				,pr.CreatedDate AS '@prcd'
				,pr.StatusId AS '@prsi'
				,pr.TypeId AS '@prti'
			FROM dbo.Nomination AS n WITH (NOLOCK)
				INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON n.NominationId = nd.NominationId
				INNER JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON nd.PerformanceRecordId = pr.PerformanceRecordId
			WHERE nd.NominatorId = @NominatorId
				AND nd.PerformanceRecordId = @PerformanceRecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @NominatorId
		,@SPName = 'Nomination_Update'
		,@ObjectId = @NominationId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END
