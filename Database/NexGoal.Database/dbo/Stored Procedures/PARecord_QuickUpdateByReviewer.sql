
CREATE PROCEDURE [dbo].[PARecord_QuickUpdateByReviewer] @ActorId INT
	,@PARecordId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181127@Oliver.Dang
Task: Quick update PA by reviewer
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

	-- Write log before update	
	SET @xml = (
			SELECT PARecordId AS '@id'
				,GradeId AS '@gid'
				,Comment AS '@comment'
				,ModifiedBy AS '@modifiedby'
				,ModifiedDate AS '@modifiedDate'
			FROM dbo.PARecord WITH (NOLOCK)
			WHERE PARecordId = @PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.PARecord WITH (ROWLOCK, UPDLOCK)
	SET ModifiedBy = @ActorId
		,ModifiedDate = GETDATE()
	WHERE PARecordId = @PARecordId

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to update performance appraisal.'

		RETURN
	END

	-- write log after insert
	SET @xml = (
			SELECT PARecordId AS '@id'
				,GradeId AS '@gid'
				,Comment AS '@comment'
				,ModifiedBy AS '@modifiedby'
				,ModifiedDate AS '@modifiedDate'
			FROM dbo.PARecord WITH (NOLOCK)
			WHERE PARecordId = @PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
		--EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		--	,@SPName = 'PARecord_UpdateByReviewer'
		--	,@ObjectId = @PerformanceRecordId
		--	,@Data = @logData
		--	,@OldData = @logDataOld
		--	,@Duration = @Duration
END
