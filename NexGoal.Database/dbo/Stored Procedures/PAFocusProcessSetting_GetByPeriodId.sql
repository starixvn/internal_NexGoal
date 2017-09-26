


CREATE PROCEDURE [dbo].[PAFocusProcessSetting_GetByPeriodId] @PeriodId INT
AS
/*
Created: 20160830@Razor.Nguyen
Task: FOcus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT PeriodId
		,[ProcessId]
		,[Duration]
	FROM [dbo].[PAFocusProcessSetting] WITH (NOLOCK)
	WHERE [PeriodId] = @PeriodId
END