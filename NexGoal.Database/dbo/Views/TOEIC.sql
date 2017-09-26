

CREATE VIEW [dbo].[TOEIC]
AS
SELECT EmpId AS EmployeeId, Listening, Reading,LRExamDate AS ExamDate
FROM NexHRM.dbo.TOEIC WITH(NOLOCK)