CREATE OR ALTER PROCEDURE [dbo].[SyncEmployeeCalendar_Import]    
    @CalendarXml NVARCHAR(MAX),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncEmployeeCalendar_Import]    
    @CalendarXml ='<ROOT>
    <EmployeeCalendar Id='1' EmpId='1' WorkDate='2024-03-20' IsHoliday='0' IsLeaveDay='0' 
        LeaveDay='0' ModifiedDate='2024-03-20'></EmployeeCalendar>
    </ROOT>'
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DocHandler INT

    SELECT @CalendarXml = REPLACE(ISNULL(@CalendarXml,''),'&','&amp;')
    
    IF OBJECT_ID('tempdb..#EmployeeCalendar') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeCalendar
    END

    CREATE TABLE #EmployeeCalendar (        
        Id BIGINT,
        EmpId INT,
        WorkDate DATETIME,
        IsOffDay BIT,
        IsHoliday BIT,
        IsLeaveDay BIT,
        LeaveDay DECIMAL(18,1),
        ModifiedDate DATETIME
    )
 
    BEGIN TRY
        EXEC sp_xml_preparedocument @DocHandler OUTPUT, @CalendarXml

        INSERT INTO #EmployeeCalendar
        SELECT
            Id,
            EmpId,
            WorkDate,
            IsOffDay,
            IsHoliday,
            IsLeaveDay,
            CASE 
                WHEN ISNUMERIC(LeaveDay) = 1 THEN CAST(LeaveDay AS DECIMAL(18,1))
                ELSE NULL 
            END,
            ModifiedDate
        FROM OPENXML(@DocHandler, '/ROOT/EmployeeCalendar', 1) WITH (
            Id BIGINT,
            EmpId INT,
            WorkDate DATETIME,
            IsOffDay BIT,
            IsHoliday BIT,
            IsLeaveDay BIT,
            LeaveDay NVARCHAR(50),
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
     
    MERGE INTO [dbo].[EmployeeCalendar] AS Target
    USING #EmployeeCalendar AS Source
    ON Target.Id = Source.Id
    WHEN MATCHED THEN
        UPDATE SET
            Target.EmpId = Source.EmpId,
            Target.WorkDate = Source.WorkDate,
            Target.IsOffDay = Source.IsOffDay,
            Target.IsHoliday = Source.IsHoliday,
            Target.IsLeaveDay = Source.IsLeaveDay,
            Target.LeaveDay = Source.LeaveDay,
            Target.ModifiedDate = Source.ModifiedDate,
            Target.SyncDate = GETDATE()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            Id, EmpId, WorkDate, IsOffDay, IsHoliday, IsLeaveDay, 
            LeaveDay, ModifiedDate, SyncDate
        )
        VALUES (
            Source.Id, Source.EmpId, Source.WorkDate, Source.IsOffDay, Source.IsHoliday, Source.IsLeaveDay, 
            Source.LeaveDay, Source.ModifiedDate, GETDATE()
        );

    SELECT @ErrCode = 0, @ErrMsg = '';
      

    DROP TABLE #EmployeeCalendar;
END 