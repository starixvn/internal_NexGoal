





CREATE PROCEDURE [dbo].[EOQYNomination_UpdateStatus] @EOQYId INT
	,@NomineeId INT
	,@StatusId TINYINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160908@Tim.Trinh
Task: Update status nomination.
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
	SET NOCOUNT ON

	DECLARE @Date DATE

	SET @Date = CAST(GETDATE() AS DATE)

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE @Date BETWEEN StartDate
					AND EndDate
				AND EOQYId = @EOQYId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'In Board permission can update status when it finised.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE dbo.EOQYNomination WITH (ROWLOCK, UPDLOCK)
		SET ApproverId = @ModifiedBy
			,StatusId = @StatusId
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE EOQYId = @EOQYId
			AND NomineeId = @NomineeId

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH
END