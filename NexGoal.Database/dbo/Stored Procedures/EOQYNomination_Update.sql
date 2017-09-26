





CREATE PROCEDURE [dbo].[EOQYNomination_Update] @NominationId INT
	,@StatusId TINYINT
	,@NominationDetails NVARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160908@Tim.Trinh
Task: Update nomination.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
• @NominationDetails: N'
     <ROOT>
        <Question QuestionId='' Comment='' />
     </ROOT>
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Date DATETIME

	SET @Date = GETDATE()

	CREATE TABLE #NominationDetails (
		QuestionId INT
		,Comment NVARCHAR(MAX)
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@NominationDetails

		INSERT INTO #NominationDetails
		SELECT QuestionId
			,Comment
		FROM OPENXML(@DocHandler, '/ROOT/Question', 1) WITH (QuestionId INT, Comment NVARCHAR(MAX)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE dbo.EOQYNomination WITH (ROWLOCK, UPDLOCK)
		SET StatusId = @StatusId
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE NominationId = @NominationId

		UPDATE dbo.EOQYNominationDetail WITH (ROWLOCK, UPDLOCK)
		SET QuestionId = dt.QuestionId
			,Comment = dt.Comment
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		FROM #NominationDetails AS dt
		WHERE NominationId = @NominationId
			AND EOQYNominationDetail.QuestionId = dt.QuestionId

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH

	DROP TABLE #NominationDetails
END