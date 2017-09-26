


CREATE PROCEDURE [dbo].[PAFocus_Delete] @PeriodId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160830@Razor.Nguyen
Task: PA Focus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM dbo.PAFocusProcess
		WHERE PAFocusId IN (
				SELECT PAFocusId
				FROM dbo.PAFocus
				WHERE PeriodId = @PeriodId
				)

		DELETE
		FROM dbo.PAFocus
		WHERE PeriodId = @PeriodId

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete PAFocus unsuccessfully.PeriodId  = ' + Convert(VARCHAR(10), @PeriodId);
	END CATCH;
END