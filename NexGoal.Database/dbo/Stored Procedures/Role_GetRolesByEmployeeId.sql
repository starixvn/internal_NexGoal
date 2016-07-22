CREATE PROCEDURE [dbo].[Role_GetRolesByEmployeeId] @EmployeeId INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT emRo.RoleId,
                loDa.Code AS RoleCode,
                loDa.Name AS RoleName
         FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
              INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
              INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
         WHERE emRO.EmployeeId = @EmployeeId
               AND look.Code = 'SR';-- System Role
     END;
