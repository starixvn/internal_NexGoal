


/****** Object:  Stored Procedure dbo.EpaPeriod_Insert    Script Date: 7/21/2016 2:45:53 PM ******/
CREATE PROCEDURE [dbo].[PAFocusProcessSetting_Insert] @PeriodId INT
	,@Duration INT
AS
/*
Created: 20160830@Razor.nguyen
Task: Focus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.PAFocusProcessSetting (
		PeriodId
		,ProcessId
		,Duration
		)
	SELECT @PeriodId
		,3
		,@Duration
END