INSERT INTO dbo.RefResource(RefName, ResourceId, [Value], [Description], IsActive)
VALUES ('SystemRole', 1, 'SA', 'System Admin', 1),
	('SystemRole', 2, 'HR', 'Human Resource', 1),
	('SystemRole', 3, 'Management', 'Management', 1),
	('SystemRole', 4, 'Evaluator', 'Evaluator', 1),
	('SystemRole', 5, 'Reviewer', 'Reviewer', 1),
	('SystemRole', 6, 'PM', 'Project Manager', 1),
	('SystemRole', 7, 'Committee', 'Committee', 1),
	('SystemRole', 101, 'Employee', 'Employee', 1)
GO

INSERT INTO dbo.RefResource(RefName, ResourceId, [Value], [Description], IsActive)
VALUES ('PerformanceGrade', 1, 'U', 'Unsatisfactory', 1),
	('PerformanceGrade', 2, 'U+', 'Unsatisfactory +', 1),
	('PerformanceGrade', 3, 'NI', 'Needs Improvement', 1),
	('PerformanceGrade', 4, 'NI+', 'Needs Improvement +', 1),
	('PerformanceGrade', 5, 'ME', 'Meets Expectations', 1),
	('PerformanceGrade', 6, 'ME+', 'Meets Expectations +', 1),
	('PerformanceGrade', 7, 'EE', 'Exceeds Expectations', 1),
	('PerformanceGrade', 8, 'EE+', 'Exceeds Expectations +', 1),
	('PerformanceGrade', 9, 'E', 'Exceptional', 1)
GO

INSERT INTO dbo.RefResource(RefName, ResourceId, [Value], [Description], IsActive)
VALUES ('PerformanceStatus', 1, 'Not Generated', '', 1),
	('PerformanceStatus', 2, 'Opened', '', 1),
	('PerformanceStatus', 3, 'In Progress', '', 1),
	('PerformanceStatus', 4, 'Closed', '', 1),
	('PerformanceStatus', 5, 'Rejected', '', 1)
GO

INSERT INTO dbo.RefResource(RefName, ResourceId, [Value], [Description], IsActive)
VALUES ('ProbationDuration', 1, '60', 'total probation days', 1),
	('ProbationRemindDay', 1, '7', 'remind before x days', 1)