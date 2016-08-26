/****** Object:  Stored Procedure dbo.FlowTemplate_Delete    Script Date: 7/21/2016 2:45:55 PM ******/

CREATE PROCEDURE [dbo].[FlowTemplate_Delete] @FlowTemplateId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20150807@Tim.Trinh
Task: Delete Flow Template.
DB: SMSEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog] @ObjectId = @FlowTemplateId, @ObjectType = 6

	BEGIN TRY
		BEGIN TRANSACTION;

		DELETE
		FROM dbo.FlowTemplate
		WHERE FlowTemplateId = @FlowTemplateId
		
		DELETE
		FROM dbo.SettingDepartmentFlow
		WHERE FlowTemplateId = @FlowTemplateId
		
		DELETE
		FROM dbo.SettingEmployeeFlow
		WHERE FlowTemplateId = @FlowTemplateId

		COMMIT TRANSACTION;

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy, @SpName = 'FlowTemplate_Delete', @ObjectId = @FlowTemplateId, @ObjectTypeId = 6, @Data = NULL, @OldData = @OldData, @ObjectIds = NULL


		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete Flow Tempalte unsuccessfully.'
	END CATCH
END
