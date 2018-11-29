


CREATE VIEW [dbo].[TOEIC]
AS
SELECT EmpId
	,MinimumScore
	,InputDate
	,CAST(LRExamDate AS DATE) AS LRExamDate
	,Listening
	,Reading
FROM [$(NexHRM)].dbo.TOEIC WITH (NOLOCK)