


CREATE PROCEDURE [dbo].[Violation_UpdateActive] @ViolationId INT
	,@ModifiedBy INT
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
		SET IsActive = ~IsActive
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE ViolationId = @ViolationId
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Change Active unsuccessfully.'
	END CATCH
END