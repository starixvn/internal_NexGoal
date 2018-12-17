
CREATE PROCEDURE [dbo].[Nomination_Update] @NominationPeriodId INT
	,@NomineeId INT
	,@NominatorId INT
	,@Comment NVARCHAR(MAX) = NULL
	,@PerformanceRecordId INT
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

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML
		,@NominationId INT
		,@CurrentNomineeId INT
		,@CurrentPeriodId INT
		,@Nominated TINYINT = 1
		,@RoleId TINYINT = 0
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SET @xml = (
			SELECT n.NominationId AS '@ni'
				,n.EOQYId AS '@eoqyi'
				,n.NomineeId AS '@nei'
				,n.StatusId AS '@si'
				,nd.NominatorId AS '@nri'
				,nd.PerformanceRecordId AS '@pri'
				,pr.Comment AS '@prc'
				,pr.ModifiedDate AS '@prmd'
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

	IF @NomineeId = @NominatorId
	BEGIN
		SET @RoleId = @SelfProcessId
	END
	ELSE
	BEGIN
		SELECT @RoleId = (CASE ProcessId
				WHEN @EvaluatorProcessId THEN @EvaluatorProcessId
				WHEN @ReviewerProcessId THEN @ReviewerProcessId
				ELSE @PMProcessId END
				)
		FROM dbo.func_GetMember(@NominatorId)
		WHERE EmployeeId = @NomineeId
			AND ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)

		SET @RoleId = ISNULL(@RoleId, 0)
	END

	UPDATE dbo.PerformanceRecord WITH (ROWLOCK, UPDLOCK)
	SET EmployeeIds = @NomineeId
		,Comment = @Comment
		,ModifiedDate = GETDATE()
		,RoleId = @RoleId
	WHERE PerformanceRecordId = @PerformanceRecordId

	SELECT @NominationId = n.NominationId
		,@CurrentNomineeId = n.NomineeId
		,@CurrentPeriodId = n.EOQYId
	FROM dbo.Nomination AS n WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.NominationDetail AS nd WITH (NOLOCK)
			WHERE nd.NominationId = n.NominationId
				AND nd.PerformanceRecordId = @PerformanceRecordId
				AND nd.NominatorId = @NominatorId
			)

	IF @CurrentNomineeId <> @NomineeId
		OR @CurrentPeriodId <> @NominationPeriodId
	BEGIN
		DECLARE @NewNominationId INT

		SELECT @NewNominationId = n.NominationId
		FROM dbo.Nomination AS n WITH (NOLOCK)
		WHERE n.NomineeId = @NomineeId
			AND n.EOQYId = @NominationPeriodId

		IF @NewNominationId IS NULL
		BEGIN
			IF NOT EXISTS (
					SELECT 1
					FROM dbo.NominationDetail AS nd WITH (NOLOCK)
					WHERE nd.NominationId = @NominationId
						AND nd.PerformanceRecordId <> @PerformanceRecordId
						AND nd.NominatorId <> @NominatorId
					)
			BEGIN
				UPDATE dbo.Nomination WITH (ROWLOCK, UPDLOCK)
				SET NomineeId = @NomineeId
					,EOQYId = @NominationPeriodId
				WHERE NominationId = @NominationId
			END
			ELSE
			BEGIN
				INSERT INTO dbo.Nomination (
					EOQYId
					,NomineeId
					,StatusId
					)
				VALUES (
					@NominationPeriodId
					,@NomineeId
					,@Nominated
					)

				SET @NewNominationId = SCOPE_IDENTITY()

				UPDATE dbo.NominationDetail WITH (ROWLOCK, UPDLOCK)
				SET NominationId = @NewNominationId
				WHERE PerformanceRecordId = @PerformanceRecordId
					AND NominatorId = @NominatorId
			END
		END
		ELSE
		BEGIN
			UPDATE dbo.NominationDetail WITH (ROWLOCK, UPDLOCK)
			SET NominationId = @NewNominationId
			WHERE PerformanceRecordId = @PerformanceRecordId
				AND NominatorId = @NominatorId

			IF NOT EXISTS (
					SELECT 1
					FROM dbo.NominationDetail AS nd WITH (NOLOCK)
					WHERE nd.NominationId = @NominationId
					)
			BEGIN
				DELETE
				FROM dbo.Nomination
				WHERE NominationId = @NominationId
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
				,pr.ModifiedDate AS '@prmd'
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