/****** Object:  Stored Procedure dbo.EpaTargetTemplateDepartment_Delete    Script Date: 7/21/2016 2:45:54 PM ******/

CREATE PROCEDURE [dbo].[EpaTargetTemplateDepartment_Delete] @TargetTemplateId INT
	,@DepartmentId INT
	,@IsGroup BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 2015-06-12@Josh.Phan
Task: Delete target template setting of employee.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Data NVARCHAR(MAX)
	DECLARE @OldData NVARCHAR(MAX)

	EXEC @OldData = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Department] @DepartmentId = @DepartmentId
		,@IsGroup = @IsGroup

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE
		FROM [dbo].[TargetTemplateDepartment]
		WHERE [TargetTemplateId] = @TargetTemplateId
			AND [DepartmentId] = @DepartmentId
			AND [IsGroup] = @IsGroup

		COMMIT TRANSACTION

		EXEC @Data = [dbo].[fncGetDataForWriteLog_TargetTemplateSetting_Department] @DepartmentId = @DepartmentId
			,@IsGroup = @IsGroup

		EXEC [dbo].[LogDetail_Insert] @ActorId = @ModifiedBy
			,@SpName = 'EpaTargetTemplateDepartment_Delete'
			,@ObjectId = NULL
			,@ObjectTypeId = 7
			,@Data = @Data
			,@OldData = @OldData
			,@ObjectIds = NULL
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete target template setting of department unsuccessfully.'
	END CATCH
END
