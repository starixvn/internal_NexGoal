CREATE OR ALTER PROCEDURE [dbo].[SyncWorkingTimeRequest_Import]    
    @WorkingTimeRequestXml NVARCHAR(MAX),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncWorkingTimeRequest_Import]    
    @WorkingTimeRequestXml ='<ROOT>
    <WorkingTimeRequest Id='1' EmpId='1' RequestType='1' Status='1' 
        FromDate='2024-03-20' ToDate='2024-03-21' TotalDays='1.5' ModifiedDate='2024-03-20'></WorkingTimeRequest>
    </ROOT>'
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DocHandler INT

    SELECT @WorkingTimeRequestXml = REPLACE(ISNULL(@WorkingTimeRequestXml,''),'&','&amp;')
    
    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
    BEGIN
        DROP TABLE #WorkingTimeRequest
    END

    CREATE TABLE #WorkingTimeRequest (        
        Id BIGINT,
        EmpId INT,
        RequestType SMALLINT,
        Status TINYINT,
        FromDate DATETIME,
        ToDate DATETIME,
        TotalDays NUMERIC(5,1),
        ModifiedDate DATETIME
    )
 
    BEGIN TRY
        EXEC sp_xml_preparedocument @DocHandler OUTPUT, @WorkingTimeRequestXml

        INSERT INTO #WorkingTimeRequest
        SELECT
            Id,
            EmpId,
            RequestType,
            Status,
            FromDate,
            ToDate,
            TotalDays,
            ModifiedDate
        FROM OPENXML(@DocHandler, '/ROOT/WorkingTimeRequest', 1) WITH (
            Id BIGINT,
            EmpId INT,
            RequestType SMALLINT,
            Status TINYINT,
            FromDate DATETIME,
            ToDate DATETIME,
            TotalDays NUMERIC(5,1),
            ModifiedDate DATETIME
        )

        EXEC sp_xml_removedocument @DocHandler
    END TRY
    BEGIN CATCH
        IF @DocHandler IS NOT NULL
            EXEC sp_xml_removedocument @DocHandler

        SELECT @ErrCode = ERROR_NUMBER()
            ,@ErrMsg = ERROR_MESSAGE()
        RETURN
    END CATCH;
     
    BEGIN TRY
        MERGE INTO [dbo].[WorkingTimeRequest] AS Target
        USING #WorkingTimeRequest AS Source
        ON Target.Id = Source.Id
        WHEN MATCHED THEN
            UPDATE SET
                Target.EmpId = Source.EmpId,
                Target.RequestType = Source.RequestType,
                Target.Status = Source.Status,
                Target.FromDate = Source.FromDate,
                Target.ToDate = Source.ToDate,
                Target.TotalDays = Source.TotalDays,
                Target.ModifiedDate = Source.ModifiedDate,
                Target.SyncDate = GETDATE()
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (
                Id, EmpId, RequestType, Status, FromDate, ToDate, TotalDays, ModifiedDate, SyncDate
            )
            VALUES (
                Source.Id, Source.EmpId, Source.RequestType, Source.Status, 
                Source.FromDate, Source.ToDate, Source.TotalDays, Source.ModifiedDate, GETDATE()
            );

        SELECT @ErrCode = 0, @ErrMsg = '';
    END TRY
    BEGIN CATCH
        SELECT @ErrCode = ERROR_NUMBER()
            ,@ErrMsg = ERROR_MESSAGE()
    END CATCH;

    IF OBJECT_ID('tempdb..#WorkingTimeRequest') IS NOT NULL
        DROP TABLE #WorkingTimeRequest;
END 