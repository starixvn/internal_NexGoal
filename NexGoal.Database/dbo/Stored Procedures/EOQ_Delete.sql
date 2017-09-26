



CREATE PROCEDURE [dbo].[EOQ_Delete] @EOQYId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160906@Tim.Trinh
Task: Delete EOQ.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATE

	SET @Date = CAST(GETDATE() AS DATE)

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE EOQYId = @EOQYId
				AND IsQuarter = 1
				AND StartDate <= @Date
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Cannot delete an "In Progress" or "Closed" nomination period.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE dt
		FROM dbo.EOQYNominationDetail AS dt
			INNER JOIN dbo.EOQYNomination AS nomination ON dt.NominationId = nomination.NominationId
			INNER JOIN dbo.EOQY AS eoq ON nomination.EOQYId = eoq.EOQYId
			AND eoq.IsQuarter = 1
		WHERE eoq.EOQYId = @EOQYId

		DELETE nomination
		FROM dbo.EOQYNomination AS nomination
			INNER JOIN dbo.EOQY AS eoq ON nomination.EOQYId = eoq.EOQYId
			AND eoq.IsQuarter = 1
		WHERE eoq.EOQYId = @EOQYId

		DELETE setting
		FROM dbo.EOQYNominatorSetting AS setting
			INNER JOIN dbo.EOQY AS eoq ON setting.EOQYId = eoq.EOQYId
		WHERE eoq.EOQYId = @EOQYId
			AND eoq.IsQuarter = 1

		DELETE
		FROM dbo.EOQY
		WHERE EOQYId = @EOQYId
			AND IsQuarter = 1

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete unsuccessfully.'
	END CATCH
END