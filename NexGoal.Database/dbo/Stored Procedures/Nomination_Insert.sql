CREATE PROCEDURE [dbo].[Nomination_Insert] @NominationPeriodId INT
	,@NomineeId INT
	,@NominatorId INT
	,@Comment NVARCHAR(MAX) = NULL
	,@PerformanceRecordId INT OUTPUT
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
				INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON nd.NominationId = nd.NominationId
			WHERE nd.NominatorId = @NominatorId
				AND n.NomineeId = @NomineeId
				AND n.EOQYId = @NominationPeriodId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The Employee has already been nominated in this period'

		RETURN
	END

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML
	DECLARE @NominationId INT

	SELECT @NominationId = n.NominationId
	FROM dbo.Nomination AS n WITH (NOLOCK)
	WHERE n.NomineeId = @NomineeId
		AND n.EOQYId = @NominationPeriodId

	IF @NominationId IS NULL
	BEGIN
		INSERT INTO dbo.Nomination (
			EOQYId
			,NomineeId
			,StatusId
			)
		VALUES (
			@NominationPeriodId
			,@NomineeId
			,1
			)

		SELECT @NominationId = SCOPE_IDENTITY()
	END

	INSERT INTO dbo.PerformanceRecord (
		EmployeeIds
		,CreatorId
		,Comment
		,StatusId
		,CreatedDate
		,TypeId
		)
	VALUES (
		@NomineeId
		,@NominatorId
		,@Comment
		,3
		,GETDATE()
		,3
		)

	SET @PerformanceRecordId  = SCOPE_IDENTITY()

	INSERT INTO dbo.NominationDetail (
		NominationId
		,NominatorId
		,PerformanceRecordId
		)
	VALUES (
		@NominationId
		,@NominatorId
		,@PerformanceRecordId
		)

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
			WHERE n.EOQYId = @NominationPeriodId
				AND n.NomineeId = @NomineeId
				AND nd.NominatorId = @NominatorId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @NominatorId
		,@SPName = 'Nomination_Insert'
		,@ObjectId = @NominationId
		,@Data = @logData
		,@Duration = @Duration
END
