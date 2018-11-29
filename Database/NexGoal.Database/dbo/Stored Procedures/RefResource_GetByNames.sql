CREATE PROCEDURE [dbo].[RefResource_GetByNames] @RefNames VARCHAR(4000)
	,@IsActive BIT = NULL
AS
/*
Created: 20180621@Tim.Trinh
Task: Get reference resource by names
DB: bodb_NexGoal
Original:

Revisions:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblRefName (RefName VARCHAR(100))

	IF @RefNames IS NOT NULL
	BEGIN
		INSERT INTO #tblRefName (RefName)
		SELECT val
		FROM dbo.func_SplitString(@RefNames, ',')
	END

	SELECT ref.ResourceId
		,ref.RefName
		,ref.Value
		,ref.[Description]
		,ref.IsActive
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		INNER JOIN #tblRefName AS tbl WITH (NOLOCK) ON ref.RefName = tbl.RefName
	WHERE ISNULL(@IsActive, ref.IsActive) = ref.IsActive
	ORDER BY ref.IsActive DESC
		,ref.Value

	DROP TABLE #tblRefName
END

