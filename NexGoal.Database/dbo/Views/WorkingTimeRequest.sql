

CREATE VIEW [dbo].[WorkingTimeRequest]
AS
SELECT EmpId
	  ,RequestType
	  ,FromDate
	  ,ToDate
	  ,TotalDays
	  ,[Status]
FROM [$(NexHRM)].dbo.WorkingTimeRequest WITH (NOLOCK)