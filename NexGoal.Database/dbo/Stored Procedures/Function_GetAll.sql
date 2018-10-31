
CREATE PROCEDURE [dbo].[Function_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20180628@Tim.Trinh
Task: Get all function permission
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT FunctionId
		,[Name]
		,FunctionKey
		,ParentId
		,DisplayOrder
		,IsActive
	FROM dbo.[Function] WITH (NOLOCK)
	WHERE ISNULL(@IsActive, IsActive) = IsActive
	ORDER BY DisplayOrder
END
