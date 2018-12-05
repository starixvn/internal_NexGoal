
/****** Object:  Stored Procedure dbo.EpaPeriod_UpdateProcessSetting    Script Date: 7/21/2016 2:45:54 PM ******/
CREATE PROCEDURE [dbo].[Period_UpdateProcessSetting] @PeriodId INT
	,@Settings NVARCHAR(MAX)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180806@Elton.Ton
Task: Update period process setting.
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)
		,@xml XML
		,@docHandler INT

	EXEC sp_xml_preparedocument @docHandler OUTPUT
		,@Settings

	SET @xml = (
			SELECT PeriodId '@pi'
				,ProcessId '@pri'
				,Duration '@d'
				,NoticeAtBegin '@nab'
				,RemindDay '@rd'
				,ModifiedBy '@mb'
				,ModifiedDate '@md'
			FROM dbo.PeriodProcessSetting WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE pps WITH (ROWLOCK, UPDLOCK)
	SET pps.Duration = setting.Duration
		,pps.NoticeAtBegin = setting.NoticeAtBegin
		,pps.RemindDay = setting.RemindDay
		,pps.ModifiedBy = @ModifiedBy
		,pps.ModifiedDate = GETDATE()
	FROM dbo.PeriodProcessSetting pps WITH (ROWLOCK, UPDLOCK)
		INNER JOIN (
		SELECT PeriodId
			,ProcessId
			,Duration
			,NoticeAtBegin
			,RemindDay
		FROM OPENXML(@docHandler, '/Root/Row', 1) WITH (PeriodId INT, ProcessId INT, Duration INT, NoticeAtBegin BIT, RemindDay INT, FocusDay INT)
		) setting ON pps.PeriodId = setting.PeriodId
		AND pps.ProcessId = setting.ProcessId

	SET @xml = (
			SELECT PeriodId '@pi'
				,ProcessId '@pri'
				,Duration '@d'
				,NoticeAtBegin '@nab'
				,RemindDay '@rd'
				,ModifiedBy '@mb'
				,ModifiedDate '@md'
			FROM dbo.PeriodProcessSetting WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_UpdateProcessSetting'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@OldData = @logOldData
		,@Duration = @Duration
END
