/****** Object:  Stored Procedure dbo.Sync_TeamSupervisor_GetMaxModifiedDate    Script Date: 7/21/2016 2:46:02 PM ******/




CREATE PROCEDURE [dbo].[Sync_TeamSupervisor_GetMaxModifiedDate]
AS
/*
Created: 20150921@Quinn.Vo
Task: Get max modified date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ModifiedDate DATETIME

	SELECT @ModifiedDate  = ISNULL(MAX(ModifiedDate), '2010-01-01')
	FROM dbo.TeamDisciplinePermission WITH (NOLOCK)

	SELECT ISNULL(@ModifiedDate , '2010-01-01')  AS ModifiedDate
END