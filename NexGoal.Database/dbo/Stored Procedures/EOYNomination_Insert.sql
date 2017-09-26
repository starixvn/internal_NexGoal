





CREATE PROCEDURE [dbo].[EOYNomination_Insert] @EOQYId INT
	,@NomineeId INT
	,@NominatorId INT
	,@StatusId TINYINT
	,@NominationDetails NVARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160908@Tim.Trinh
Task: insert nomination.
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

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQYNomination
			WHERE EOQYId = @EOQYId
				AND NomineeId = @NomineeId
				AND NominatorId = @NominatorId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'You have nominated for this nominee.'

		RETURN
	END

	DECLARE @Date DATETIME
		,@NominationId INT

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
			,@ErrMsg = N'Insert unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO dbo.EOQYNomination (
			EOQYId
			,NomineeId
			,NominatorId
			,ApproverId
			,StatusId
			,ModifiedBy
			,ModifiedDate
			)
		VALUES (
			@EOQYId
			,@NomineeId
			,@NominatorId
			,- 1
			,@StatusId
			,@ModifiedBy
			,@Date
			)

		SELECT @NominationId = SCOPE_IDENTITY()

		INSERT INTO dbo.EOQYNominationDetail (
			NominationId
			,QuestionId
			,Comment
			,ModifiedBy
			,ModifiedDate
			)
		SELECT DISTINCT @NominationId
			,QuestionId
			,Comment
			,@ModifiedBy
			,@Date
		FROM #NominationDetails

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert unsuccessfully.'
	END CATCH

	DROP TABLE #NominationDetails
END