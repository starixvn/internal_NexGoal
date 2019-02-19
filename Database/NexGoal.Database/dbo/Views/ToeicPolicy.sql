





CREATE VIEW [dbo].[ToeicPolicy]
AS
WITH PolicyScore (
	[Left]
	,[Right]
	,Score
	)
AS (
	--SELECT - 990, - 105, - 3
	--UNION
	--SELECT - 100, - 55, - 2
	--UNION
	SELECT - 990, - 5, - 1
	UNION
	--SELECT 105, 145, 1
	--UNION
	--SELECT 150, 195, 2
	--UNION
	SELECT 205, 990, 1
	)
--------------------
SELECT emp.EmployeeId
	,(CASE 
		WHEN (toeic.Reading + toeic.Listening) > 900 THEN 1
		WHEN MONTH(GETDATE()) BETWEEN 1
				AND 6
			AND DATEADD(YEAR, 3, toeic.LRExamDate) >= CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-05-01' AS DATETIME) THEN ISNULL(ps.Score, 0)
		WHEN MONTH(GETDATE()) BETWEEN 7
				AND 12
			AND DATEADD(YEAR, 3, toeic.LRExamDate) >= CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-11-01' AS DATETIME) THEN ISNULL(ps.Score, 0)
		WHEN MONTH(GETDATE()) BETWEEN 1
				AND 6
			AND CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-05-01' AS DATETIME) > toeic.InputDate THEN - 1
		WHEN MONTH(GETDATE()) BETWEEN 7
				AND 12
			AND CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-11-01' AS DATETIME) > toeic.InputDate THEN - 1
		ELSE ISNULL(ps.Score, 0) END
		) AS Score
FROM dbo.Employee AS emp WITH (NOLOCK)
	LEFT JOIN dbo.TOEIC AS toeic WITH (NOLOCK) ON emp.EmployeeId = toeic.EmpId
	LEFT JOIN PolicyScore AS ps ON (toeic.Reading + toeic.Listening - toeic.MinimumScore) BETWEEN ps.[Left]
		AND ps.[Right]
--WHERE emp.GroupId NOT IN (1, 2, 12, 38) -- FA, ITNW, MGT, EXT, HR