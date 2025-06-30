CREATE OR ALTER   PROCEDURE [dbo].[SyncEmployeeOvertime_Import]    
    @OvertimeXml NVARCHAR(MAX),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncEmployeeOvertime_Import]    
    @OvertimeXml ='<ROOT>
    <EmployeeOvertime OvertimeId='1' EmployeeId='1' Date='2024-03-20' 
        Hour='2.5' ModifiedDate='2024-03-20'></EmployeeOvertime>
    </ROOT>'
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DocHandler INT

    SELECT @OvertimeXml = REPLACE(ISNULL(@OvertimeXml,''),'&','&amp;')
    
    IF OBJECT_ID('tempdb..#EmployeeOvertime') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeOvertime
    END

    CREATE TABLE #EmployeeOvertime (        
        OvertimeId INT,
        EmployeeId INT,
        Date DATETIME,
        Hour DECIMAL(18,1),
        ModifiedDate DATETIME
    )
 
    BEGIN TRY
        EXEC sp_xml_preparedocument @DocHandler OUTPUT, @OvertimeXml

        INSERT INTO #EmployeeOvertime
        SELECT
            OvertimeId,
            EmployeeId,
            Date,
            CASE 
                WHEN ISNUMERIC(Hour) = 1 THEN CAST(Hour AS DECIMAL(18,1))
                ELSE NULL 
            END,
            ModifiedDate
        FROM OPENXML(@DocHandler, '/ROOT/EmployeeOvertime', 1) WITH (
            OvertimeId INT,
            EmployeeId INT,
            Date DATETIME,
            Hour NVARCHAR(50),
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
        MERGE INTO [dbo].[EmployeeOvertime] AS Target
        USING #EmployeeOvertime AS Source
        ON Target.OvertimeId = Source.OvertimeId 
            AND Target.EmployeeId = Source.EmployeeId
            AND Target.Date = Source.Date
        WHEN MATCHED THEN
            UPDATE SET
                Target.Hour = Source.Hour,
                Target.ModifiedDate = Source.ModifiedDate,
                Target.SyncDate = GETDATE()
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (
                OvertimeId, EmployeeId, Date, Hour, 
                ModifiedDate, SyncDate
            )
            VALUES (
                Source.OvertimeId, Source.EmployeeId, Source.Date, Source.Hour, 
                Source.ModifiedDate, GETDATE()
            );

        SELECT @ErrCode = 0, @ErrMsg = '';
    END TRY
    BEGIN CATCH
        SELECT @ErrCode = ERROR_NUMBER()
            ,@ErrMsg = ERROR_MESSAGE()
    END CATCH;

    IF OBJECT_ID('tempdb..#EmployeeOvertime') IS NOT NULL
        DROP TABLE #EmployeeOvertime;
END 