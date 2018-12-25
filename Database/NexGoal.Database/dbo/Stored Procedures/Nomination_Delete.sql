
CREATE PROCEDURE [dbo].[Nomination_Delete] @PerformanceRecordId INT
	,@DeletedById INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181122@Elton.Ton
Task: delete a nomination
DB: bodb_NexGoal
Original: 
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML
		,@NominationId INT

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
			WHERE nd.PerformanceRecordId = @PerformanceRecordId
			FOR XML PATH('r')
				,TYPE
			)

	DELETE
	FROM dbo.PerformanceRecord
	WHERE PerformanceRecordId = @PerformanceRecordId

	SELECT @NominationId = NominationId
	FROM dbo.NominationDetail WITH (NOLOCK)
	WHERE PerformanceRecordId = @PerformanceRecordId

	DELETE
	FROM dbo.NominationDetail
	WHERE PerformanceRecordId = @PerformanceRecordId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.NominationDetail WITH (NOLOCK)
			WHERE NominationId = @NominationId
			)
	BEGIN
		DELETE
		FROM dbo.Nomination
		WHERE NominationId = @NominationId
	END

	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @DeletedById
		,@SPName = 'Nomination_Delete'
		,@ObjectId = @PerformanceRecordId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END