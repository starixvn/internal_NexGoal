


CREATE PROCEDURE [dbo].[Violation_Update] @ViolationId INT
	,@Description NVARCHAR(max)
	,@ModifiedBy INT
	,@EffectedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160901@Razor.Nguyen
Task: Violation management.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		UPDATE dbo.Violation
		SET [EffectedDate] = @EffectedDate
			,[Description] = @Description
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE ViolationId = @ViolationId
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert unsuccessfully.'
	END CATCH
END