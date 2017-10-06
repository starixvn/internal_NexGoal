
CREATE PROCEDURE [dbo].[EOQ_UpdateNominatorSettingById] @EOQYId INT
	,@Details NVARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160907@Tim.Trinh
Task: Update nominator setting.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
• @Details: N'
     <ROOT>
        <NominatorSetting RoleId='' NominatorId='' />
     </ROOT>
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #tblNominatorSetting (
		RoleId NVARCHAR(10)
		,NominatorId INT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@Details

		INSERT INTO #tblNominatorSetting (
			RoleId
			,NominatorId
			)
		SELECT RoleId
			,NominatorId
		FROM OPENXML(@DocHandler, '/ROOT/NominatorSetting', 1) WITH (RoleId NVARCHAR(10), NominatorId INT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE setting
		FROM dbo.EOQYNominatorSetting AS setting
		WHERE setting.EOQYId = @EOQYId
			AND EXISTS (
				SELECT 1
				FROM #tblNominatorSetting WITH (NOLOCK)
				WHERE setting.NominatorId = NominatorId
				)

		INSERT INTO dbo.EOQYNominatorSetting (
			EOQYId
			,RoleId
			,NominatorId
			,ModifiedBy
			,ModifiedDate
			)
		SELECT @EOQYId
			,RoleId
			,NominatorId
			,@ModifiedBy
			,GETDATE()
		FROM #tblNominatorSetting WITH (NOLOCK)
		WHERE ISNULL(RoleId, '') <> ''

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH

	DROP TABLE #tblNominatorSetting
END