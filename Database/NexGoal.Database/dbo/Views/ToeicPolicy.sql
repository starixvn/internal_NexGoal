
CREATE VIEW [dbo].[ToeicPolicy]
AS
WITH PolicyScore (
	[Left]
	,[Right]
	,Score
	)
AS (
	SELECT - 990
		,- 5
		,- 1
	
	UNION
	SELECT 200
		,990
		,1
	)
--------------------
SELECT emp.EmployeeId
	,(CASE 
		WHEN ps.Score > 0
			AND DATEADD(YEAR, 3, toeic.LRExamDate) < GETDATE() THEN 0
		WHEN (
				MONTH(GETDATE()) = 12
				OR MONTH(GETDATE()) BETWEEN 1
					AND 5
				) THEN CASE 
				WHEN CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-05-01' AS DATETIME) >= toeic.InputDate
					OR CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-05-01' AS DATETIME) <= toeic.LRExamDate
					OR DATEADD(YEAR, 3, toeic.LRExamDate) < CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-05-31' AS DATETIME) THEN - 1
				WHEN toeic.LRExamDate < CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-05-01' AS DATETIME)
					AND DATEADD(YEAR, 3, toeic.LRExamDate) >= CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-05-31' AS DATETIME) THEN ISNULL(ps.Score, 0)
				END
		WHEN (
				MONTH(GETDATE()) BETWEEN 6
					AND 11
				) THEN CASE 
				WHEN CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-11-01' AS DATETIME) >= toeic.InputDate
					OR CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-11-01' AS DATETIME) <= toeic.LRExamDate
					OR DATEADD(YEAR, 3, toeic.LRExamDate) < CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-11-30' AS DATETIME) THEN - 1
				WHEN toeic.LRExamDate < CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-11-01' AS DATETIME)
					AND DATEADD(YEAR, 3, toeic.LRExamDate) >= CAST(CAST(YEAR(GETDATE()) AS NVARCHAR(5)) + '-11-30' AS DATETIME) THEN ISNULL(ps.Score, 0)
				END
		ELSE 0 END
		) AS Score
FROM dbo.Employee AS emp WITH (NOLOCK)
	LEFT JOIN dbo.TOEIC AS toeic WITH (NOLOCK) ON emp.EmployeeId = toeic.EmpId
	LEFT JOIN PolicyScore AS ps ON (toeic.Reading + toeic.Listening - toeic.MinimumScore) BETWEEN ps.[Left]
		AND ps.[Right]