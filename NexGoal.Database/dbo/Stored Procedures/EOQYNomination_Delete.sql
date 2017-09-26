




CREATE PROCEDURE [dbo].[EOQYNomination_Delete] @NominationId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160908@Tim.Trinh
Task: Delete nomination.
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

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQYNomination WITH (NOLOCK)
			WHERE NominationId = @NominationId
				AND StatusId > 1 -- nominated
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Can''t delete nomination.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM dbo.EOQYNomination
		WHERE NominationId = @NominationId

		DELETE
		FROM dbo.EOQYNominationDetail
		WHERE NominationId = @NominationId

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete unsuccessfully.'
	END CATCH
END