/****** Object:  Stored Procedure dbo.Sync_EmployeeDiscipline_GetMaxModifiedDate    Script Date: 7/21/2016 2:46:00 PM ******/


CREATE PROCEDURE [dbo].[Sync_EmployeeDiscipline_GetMaxModifiedDate]
AS
/*
Created: 20150914@Quinn.Vo
Task: Get max modified date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ModifiedDate DATETIME

	SELECT @ModifiedDate  = ISNULL(MAX(ModifiedDate), '2015-01-01')
	FROM dbo.EmployeeDiscipline WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate , '2015-01-01')  AS ModifiedDate
END