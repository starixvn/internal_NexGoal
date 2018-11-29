
CREATE PROCEDURE [dbo].[PerformanceRecord_Delete] @PerformanceRecordId INT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181121@Tim.Trinh
Task: delete performance record
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Draft TINYINT = 2
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- Write log before delete
	SET @xml = (
			SELECT PerformanceRecordId AS '@id'
				,EmployeeIds AS '@eids'
				,CreatorId AS '@cid'
				,GradeId AS '@gid'
				,Comment AS '@comment'
				,StatusId AS '@sid'
				,ProjectIds AS '@pids'
				,CreatedDate AS '@cdate'
				,TypeId AS '@type'
				,RoleId AS '@role'
			FROM dbo.PerformanceRecord WITH (NOLOCK)
			WHERE PerformanceRecordId = @PerformanceRecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord WITH (NOLOCK)
			WHERE PerformanceRecordId = @PerformanceRecordId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'This performance record is not exist.'

		RETURN
	END
	
	IF NOT EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord WITH (NOLOCK)
			WHERE PerformanceRecordId = @PerformanceRecordId
				AND CreatorId = @ActorId
				AND StatusId = @Draft
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'You can''t delete this performance record.'

		RETURN
	END

	DELETE
	FROM dbo.PerformanceRecord
	WHERE PerformanceRecordId = @PerformanceRecordId
		AND CreatorId = @ActorId
		AND StatusId = @Draft

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to delete performance.'

		RETURN
	END

	-- write log after delete
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
	
	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
			,@SPName = 'PerformanceRecord_Delete'
			,@ObjectId = @PerformanceRecordId
			,@OldData = @logDataOld
			,@Duration = @duration
END