
CREATE PROCEDURE [dbo].[Auto_PerformanceRecord_UpdateStatus] @ErrCode INT = 0 OUTPUT
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

	SELECT @PerformanceRecordDuration = Value
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'PerformanceRecordDuration'

	UPDATE record
	SET record.StatusId = @Closed
	FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
	WHERE DATEADD(DAY, @PerformanceRecordDuration, CreatedDate) < @Today
		AND StatusId = @Submitted

	IF @@ERROR > 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = 'Failed to update PA status.'

		RETURN
	END
END
