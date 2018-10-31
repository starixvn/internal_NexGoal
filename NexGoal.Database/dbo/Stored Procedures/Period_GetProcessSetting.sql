
CREATE PROCEDURE [dbo].[Period_GetProcessSetting] @PeriodId INT
AS
/*
Created: 20180727@Elton.Ton
Task: get period process setting
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT rr.ResourceId AS ProcessId
		,rr.Value AS ProcessName
		,@PeriodId AS PeriodId
		,pps.Duration
		,pps.NoticeAtBegin
		,pps.RemindDay
		,pps.FocusDay
	FROM dbo.RefResource AS rr WITH (NOLOCK)
		INNER JOIN dbo.PeriodProcessSetting AS pps WITH (NOLOCK) ON rr.ResourceId = pps.ProcessId
		AND pps.PeriodId = @PeriodId
	WHERE rr.RefName = 'PerformanceProcess'
		AND (
			pps.ProcessId = 1
			OR pps.ProcessId = 5
			)
END