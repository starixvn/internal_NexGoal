/****** Object:  User Defined Function dbo.fncGetDataForWriteLog_Role_AssignFunctionToRole    Script Date: 7/21/2016 2:45:41 PM ******/

CREATE FUNCTION [dbo].[fncGetDataForWriteLog_Role_AssignFunctionToRole] (@ObjectId INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Data NVARCHAR(MAX)
	DECLARE @Functions NVARCHAR(MAX)

	SET @Functions = ''

	SELECT @Functions = @Functions + '<func><id>' + CAST([FunctionId] AS NVARCHAR(10)) + '</id><r>' + CAST([IsRead] AS NVARCHAR) + '</r><h>' + CAST([IsHost] AS NVARCHAR) + '</h></func>'
	FROM [dbo].[RoleFunction] WITH (NOLOCK)
	WHERE [RoleId] = @ObjectId

	SELECT @Data = CONCAT ('<r><id>', CAST(@ObjectId AS NVARCHAR(10)), '</id><funcs>', @Functions, '</funcs></r>')

	RETURN @Data
END
