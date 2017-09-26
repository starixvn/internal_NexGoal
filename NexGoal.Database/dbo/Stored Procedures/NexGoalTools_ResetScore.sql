

CREATE PROCEDURE [dbo].[NexGoalTools_ResetScore] @ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(MAX) = '' OUTPUT
AS
/*
Created: 20160829@Tim.Trinh
Task: reset score
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE dbo.EPA WITH (ROWLOCK, UPDLOCK)
		SET FinalScore = NULL
			,Grade = NULL

		UPDATE dbo.EPADetail WITH (ROWLOCK, UPDLOCK)
		SET Score = NULL
			,Rate = NULL
			,IsMaxScore = 0
			
		UPDATE dbo.EPAProcess WITH (ROWLOCK, UPDLOCK)
		SET TotalScore = NULL
			,AdjustScore = NULL

		UPDATE dbo.PAFocus WITH (ROWLOCK, UPDLOCK)
		SET IsReadyPromotion = NULL
			,IsPoorPerformance = NULL
			,Contribution = NULL
			,BonusGiven = NULL

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Reset score unsuccessfully.'
	END CATCH;
END