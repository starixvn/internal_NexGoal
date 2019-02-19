



CREATE VIEW [dbo].[TOEIC]
AS
SELECT EmpId
	,MinimumScore
	,CAST(InputDate AS DATE) AS InputDate
	,CAST(LRExamDate AS DATE) AS LRExamDate
	,Listening
	,Reading
FROM NexHRM.dbo.TOEIC WITH (NOLOCK)