
CREATE PROCEDURE [dbo].[Auto_PerformanceRecord_UpdateStatus_xpre] @ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180914@Tim.Trinh
Task: Update performance status.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PerformanceRecordDuration TINYINT
		,@Submitted TINYINT = 3
		,@Closed TINYINT = 4
	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tblPerformanceRecord (Id INT)

	SELECT @PerformanceRecordDuration = Value
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'PerformanceRecordDuration'

	INSERT INTO #tblPerformanceRecord (Id)
	SELECT PerformanceRecordId
	FROM dbo.PerformanceRecord WITH (NOLOCK)
	WHERE DATEADD(DAY, @PerformanceRecordDuration, CreatedDate) < @Today
		AND StatusId = @Submitted

	-- write log before update
	SET @xml = (
			SELECT record.PerformanceRecordId AS '@id'
				,record.StatusId AS '@stid'
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				INNER JOIN #tblPerformanceRecord AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.Id
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	-- update
	UPDATE record
	SET record.StatusId = @Closed
	FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPerformanceRecord AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.Id

	-- write log after update
	SET @xml = (
			SELECT record.PerformanceRecordId AS '@id'
				,record.StatusId AS '@stid'
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				INNER JOIN #tblPerformanceRecord AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.Id
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
	
	DROP TABLE #tblPerformanceRecord
	
	--EXEC dbo.LogDetail_Insert @ActorId = - 1
	--	,@SPName = 'Auto_PerformanceRecord_UpdateStatus'
	--	,@ObjectId = - 1
	--	,@Data = @logData
	--	,@OldData = @logDataOld
	--	,@Duration = @Duration
END
