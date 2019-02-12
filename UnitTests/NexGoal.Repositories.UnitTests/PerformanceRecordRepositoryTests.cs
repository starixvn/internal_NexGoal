namespace NexGoal.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NSubstitute;
    using Xunit;

    public class PerformanceRecordRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public PerformanceRecordRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task GetPerformanceRecordByEmployeeAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee{
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Employees = new List<Employee>{
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        }
                    },
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "Name1", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 2, Name = "Name2", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 3, Name = "Name3", PerformanceRecordId = 1 },
            };
            var expected = new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>(performanceRecords, projects);

            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, Employee, NominationPeriodModel, PeriodBaseModel, PerformanceRecord>>(),
                "EmployeeId, EmployeeId, NominationPeriodId, PeriodId").Returns(performanceRecords);
            multiResultReader.Read<ProjectModel>().Returns(projects);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetPerformanceRecordByEmployeeAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPerformanceTimelineAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee
                    {
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Employees = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        }
                    },
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };

            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            objectDb.QueryAsync(Arg.Any<Func<PerformanceRecord, Employee, NominationPeriodModel, PerformanceRecord>>(),
                "EmployeeId, NominationPeriodId",
                criteria).Returns(performanceRecords);

            // Act
            var actual = await repository.GetPerformanceTimelineAsync(criteria);

            // Assert
            Assert.Equal(performanceRecords, actual);
        }

        [Fact]
        public async Task GetPerformanceRecordByIdAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee{
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Employees = new List<Employee>{
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        }
                    },
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "Name1", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 2, Name = "Name2", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 3, Name = "Name3", PerformanceRecordId = 1 },
            };
            performanceRecords[0].Projects = projects;

            var expected = performanceRecords.FirstOrDefault();

            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, Employee, PerformanceRecord>>(),
                "EmployeeId, EmployeeId").Returns(performanceRecords);
            multiResultReader.Read<ProjectModel>().Returns(projects);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetPerformanceRecordByIdAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetDraftPerformanceRecordByIdAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee{
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "Name1", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 2, Name = "Name2", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 3, Name = "Name3", PerformanceRecordId = 1 },
            };

            var employees = new List<Employee>
            {
                new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        },
                new Employee
                        {
                            EmployeeId = 3,
                            NickName = "NickName3",
                            FullName = "FullName3",
                            Code = "Code3"
                        }
            };

            performanceRecords[0].Projects = projects;
            performanceRecords[0].Employees = employees;

            var expected = performanceRecords.FirstOrDefault();

            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, PerformanceRecord>>(), "EmployeeId").Returns(performanceRecords);
            multiResultReader.Read<ProjectModel>().Returns(projects);
            multiResultReader.Read<Employee>().Returns(employees);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetDraftPerformanceRecordByIdAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPerformanceRecordByIdsAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee{
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Employees = new List<Employee>{
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        }
                    },
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "Name1", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 2, Name = "Name2", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 3, Name = "Name3", PerformanceRecordId = 1 },
            };
            var expected = new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>(performanceRecords, projects);

            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, Employee, PerformanceRecord>>(), "EmployeeId, EmployeeId").Returns(performanceRecords);
            multiResultReader.Read<ProjectModel>().Returns(projects);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetPerformanceRecordByIdsAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberPerformanceDetailAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee{
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Employees = new List<Employee>{
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        }
                    },
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };
            var employees = new List<Employee>
            {
                new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        },
                new Employee
                        {
                            EmployeeId = 3,
                            NickName = "NickName3",
                            FullName = "FullName3",
                            Code = "Code3"
                        }
            };
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "Name1", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 2, Name = "Name2", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 3, Name = "Name3", PerformanceRecordId = 1 },
            };
            var expected = new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>(performanceRecords, employees, projects);

            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, NominationPeriodModel, PeriodBaseModel, PerformanceRecord>>(), 
                    "EmployeeId, NominationPeriodId, PeriodId").Returns(performanceRecords);
            multiResultReader.Read<Employee>().Returns(employees);
            multiResultReader.Read<ProjectModel>().Returns(projects);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetMemberPerformanceDetailAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberPerformanceAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee{
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Employees = new List<Employee>{
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        }
                    },
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };
            var employees = new List<Employee>
            {
                new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        },
                new Employee
                        {
                            EmployeeId = 3,
                            NickName = "NickName3",
                            FullName = "FullName3",
                            Code = "Code3"
                        }
            };
            var projects = new List<ProjectModel>
            {
                new ProjectModel { ProjectId = 1, Name = "Name1", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 2, Name = "Name2", PerformanceRecordId = 1 },
                new ProjectModel { ProjectId = 3, Name = "Name3", PerformanceRecordId = 1 },
            };
            var expected = new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>(performanceRecords, employees, projects);

            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, PerformanceRecord>>(),
                    "EmployeeId").Returns(performanceRecords);
            multiResultReader.Read<Employee>().Returns(employees);
            multiResultReader.Read<ProjectModel>().Returns(projects);
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetMemberPerformanceAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberPerformanceTimelineAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var performanceRecords = new List<PerformanceRecord>
            {
                new PerformanceRecord
                {
                    PerformanceRecordId = 1,
                    Creator = new Employee{
                        EmployeeId = 1,
                        NickName = "NickName1",
                        FullName = "FullName1",
                        Code = "Code1"
                    },
                    GradeId = "GradeId1",
                    Employees = new List<Employee>{
                        new Employee
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        }
                    },
                    Comment = "Comment ABC",
                    IsDraft = true
                }
            };
            var members = new List<MemberPerformanceTimelineModel>
            {
                new MemberPerformanceTimelineModel
                        {
                            EmployeeId = 2,
                            NickName = "NickName2",
                            FullName = "FullName2",
                            Code = "Code2"
                        },
                new MemberPerformanceTimelineModel
                        {
                            EmployeeId = 3,
                            NickName = "NickName3",
                            FullName = "FullName3",
                            Code = "Code3"
                        }
            };

            var expected = new Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>(members, performanceRecords);

            var multiResultReader = Helpers.FakeMultipleResultReader();
            var criteria = Helpers.FakeCriteria();
            var repository = new PerformanceRecordRepository();
            multiResultReader.Read<MemberPerformanceTimelineModel>().Returns(members);
            multiResultReader.Read(
                Arg.Any<Func<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>>(),
                    "EmployeeId, EmployeeId, NominationPeriodId").Returns(performanceRecords);
            
            objectDb.QueryMultipleAsync(criteria).Returns(multiResultReader);

            // Act
            var actual = await repository.GetMemberPerformanceTimelineAsync(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
