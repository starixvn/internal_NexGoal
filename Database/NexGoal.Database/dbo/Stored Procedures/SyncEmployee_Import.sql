CREATE OR ALTER PROCEDURE [dbo].[SyncEmployee_Import]	
	@EmployeeXml NVARCHAR(MAX)

	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncEmployee_Import]	
	@UserXML ='<ROOT>
	"<Employee EmployeeId='{0}' DomainUsername='{1}' Code='{2}' FullName='{3}' NickName='{4}' GroupId='{5}' Enabled='{6}' JoinedDate='{7}' "
        + "ConfirmedDate='{8}' JobTitleId='{9}' Email='{10}' ConversationId='{11}' IsDeleted='{12}'></Employee>"
</ROOT>'

*/
BEGIN
	SET NOCOUNT ON;


	-- For write log
	DECLARE 
		@StartRun					DATETIME,
		@EndRun						DATETIME,
		@Duration					INT,
		@LogTraderData				NVARCHAR(MAX),	
		@LogTraderOldData			NVARCHAR(MAX),	
		@Xml						XML		 

	
	DECLARE @DocHandler INT

	SELECT @EmployeeXml = REPLACE(ISNULL(@EmployeeXml,''),'&','&amp;')
	
	PRINT LEN(@EmployeeXml)

	IF OBJECT_ID('tempdb..#Employee') IS NOT NULL
	BEGIN
		DROP TABLE #Employee
	END

	CREATE TABLE #Employee (		
		EmployeeId INT,
		DomainUsername VARCHAR(50),
		Code VARCHAR(20),
		FullName NVARCHAR(50),
		NickName NVARCHAR(20),
		GroupId INT,
		Enabled BIT,
		IsDeleted BIT,
		JoinedDate DATE,
		ConfirmedDate DATE,
		JobTitleId INT,
		Email VARCHAR(50),
		ConversationId VARCHAR(255),
		ModifiedDate DATETIME
	)
 
	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@EmployeeXml

		INSERT INTO #Employee
		SELECT
			EmployeeId,
			DomainUsername,
			Code,
			FullName,
			NickName,
			GroupId,
			Enabled,
			IsDeleted,
			CASE WHEN ISDATE(JoinedDate) = 1 THEN CAST(JoinedDate AS DATE) ELSE NULL END,
			CASE WHEN ISDATE(ConfirmedDate) = 1 THEN CAST(ConfirmedDate AS DATE) ELSE NULL END,
			JobTitleId,
			Email,
			ConversationId,
			ModifiedDate
		FROM OPENXML(@DocHandler, '/ROOT/Employee', 1) WITH (
			EmployeeId INT,
			DomainUsername VARCHAR(50),
			Code VARCHAR(20),
			FullName NVARCHAR(50),
			NickName NVARCHAR(20),
			GroupId INT,
			Enabled BIT,
			IsDeleted BIT,
			JoinedDate NVARCHAR(50),
			ConfirmedDate NVARCHAR(50),
			JobTitleId INT,
			Email VARCHAR(50),
			ConversationId VARCHAR(255),
			ModifiedDate DATETIME
		)

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'
		PRINT 1
		RETURN
	END CATCH;
	 
	MERGE INTO [dbo].[Employee] AS Target
	USING #Employee AS Source
	ON Target.EmployeeId = Source.EmployeeId
	WHEN MATCHED THEN
		UPDATE SET
			Target.DomainUsername = Source.DomainUsername,
			Target.Code = Source.Code,
			Target.FullName = Source.FullName,
			Target.NickName = Source.NickName,
			Target.GroupId = Source.GroupId,
			Target.Enabled = Source.Enabled,
			Target.IsDeleted = Source.IsDeleted,
			Target.JoinedDate = Source.JoinedDate,
			Target.ConfirmedDate = Source.ConfirmedDate,
			Target.JobTitleId = Source.JobTitleId,
			Target.Email = Source.Email,
			Target.ConversationId = Source.ConversationId,
			Target.ModifiedDate = Source.ModifiedDate,
			Target.SyncDate = GETDATE()
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			EmployeeId, DomainUsername, Code, FullName, NickName, GroupId, Enabled, IsDeleted,
			JoinedDate, ConfirmedDate, JobTitleId, Email, ConversationId, ModifiedDate, SyncDate
		)
		VALUES (
			Source.EmployeeId, Source.DomainUsername, Source.Code, Source.FullName, Source.NickName, Source.GroupId,
			Source.Enabled, Source.IsDeleted, Source.JoinedDate, Source.ConfirmedDate, Source.JobTitleId, Source.Email, Source.ConversationId, Source.ModifiedDate, GETDATE()
		);

	SELECT @ErrCode = 0, @ErrMsg = '';

	DROP TABLE #Employee;
END