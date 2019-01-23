namespace NexGoal.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Repositories.Interfaces;

    public class PerformanceRecordRepository : IPerformanceRecordRepository
    {
        public async Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>> GetPerformanceRecordByEmployeeAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceRecords = multi.Read<PerformanceRecord, Employee,
                            Employee, NominationPeriodModel, PeriodBaseModel, PerformanceRecord>(
                        (performanceRecord, employee, creator, nomination, period) =>
                        {
                            performanceRecord.Employees = new List<Employee> { employee };
                            performanceRecord.Creator = creator;
                            performanceRecord.NominationPeriod = nomination;
                            performanceRecord.Period = period;

                            return performanceRecord;
                        },
                         "EmployeeId, EmployeeId, NominationPeriodId, PeriodId"
                        );

                    
                    var performanceProjects = multi.Read<ProjectModel>();
                   
                    return new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>(performanceRecords, performanceProjects);
                }
            }
        }

        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceTimelineAsync(ICriteria criteria)
        {
            using (IObjectDb objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var performanceRecords = await objectDb.QueryAsync<PerformanceRecord, Employee, NominationPeriodModel, PerformanceRecord>(
                        (performanceRecord, creator, nomination) =>
                        {
                            performanceRecord.Creator = creator;
                            performanceRecord.NominationPeriod = nomination;

                            return performanceRecord;
                        },
                         "EmployeeId, NominationPeriodId",
                         criteria).ConfigureAwait(false);

                return performanceRecords;
            }
        }

        public async Task<PerformanceRecord> GetPerformanceRecordByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceRecord = multi.Read<PerformanceRecord, Employee, Employee, PerformanceRecord>(
                        (record, employee, creator) =>
                        {
                            record.Creator = creator;
                            record.Employees = new List<Employee> { employee };

                            return record;
                        },
                         "EmployeeId, EmployeeId"
                        ).FirstOrDefault();

                    performanceRecord.Projects = multi.Read<ProjectModel>();
                    return performanceRecord;
                }
            }
        }

        public async Task<PerformanceRecord> GetDraftPerformanceRecordByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceRecord = multi.Read<PerformanceRecord, Employee, PerformanceRecord>(
                        (record, creator) =>
                        {
                            record.Creator = creator;

                            return record;
                        },
                         "EmployeeId, EmployeeId"
                        ).FirstOrDefault();

                    performanceRecord.Projects = multi.Read<ProjectModel>();
                    performanceRecord.Employees = multi.Read<Employee>();

                    return performanceRecord;
                }
            }
        }

        public async Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>> GetPerformanceRecordByIdsAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceRecords = multi.Read<PerformanceRecord, Employee, Employee, PerformanceRecord>(
                        (performanceRecord, employee, creator) =>
                        {
                            performanceRecord.Employees = new List<Employee> { employee };
                            performanceRecord.Creator = creator;

                            return performanceRecord;
                        },
                         "EmployeeId, EmployeeId"
                        );

                    var performanceProjects = multi.Read<ProjectModel>();

                    return new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<ProjectModel>>(performanceRecords, performanceProjects);
                }
            }
        }

        public async Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>> GetMemberPerformanceDetailAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceRecords = multi.Read<PerformanceRecord, Employee, NominationPeriodModel, PeriodBaseModel, PerformanceRecord>(
                        (performanceRecord, creator, nomination, period) =>
                        {
                            performanceRecord.Creator = creator;
                            performanceRecord.NominationPeriod = nomination;
                            performanceRecord.Period = period;

                            return performanceRecord;
                        }, "EmployeeId, NominationPeriodId, PeriodId");

                    var performanceMembers = multi.Read<Employee>();
                    var performanceProjects = multi.Read<ProjectModel>();

                    return new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>(performanceRecords, performanceMembers, performanceProjects);
                }
            }
        }

        public async Task<Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>> GetMemberPerformanceAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceRecords = multi.Read<PerformanceRecord, Employee, PerformanceRecord>(
                        (performanceRecord, creator) =>
                        {
                            performanceRecord.Creator = creator;
                            return performanceRecord;
                        },"EmployeeId");

                    var performanceMembers = multi.Read<Employee>();
                    var performanceProjects = multi.Read<ProjectModel>();

                    return new Tuple<IEnumerable<PerformanceRecord>, IEnumerable<Employee>, IEnumerable<ProjectModel>>(performanceRecords, performanceMembers, performanceProjects);
                }
            }
        }

        public async Task<Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>> GetMemberPerformanceTimelineAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var members = multi.Read<MemberPerformanceTimelineModel>();

                    var performanceRecords = multi.Read<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>(
                        (performanceRecord, employee, creator, nomination) =>
                        {
                            performanceRecord.Employees = new List<Employee> { employee };
                            performanceRecord.Creator = creator;
                            performanceRecord.NominationPeriod = nomination;

                            return performanceRecord;
                        },
                        "EmployeeId, NominationPeriodId");

                    return new Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>(members, performanceRecords);
                }

                //var performanceRecords = await objectDb.QueryAsync<PerformanceRecord, Employee, NominationPeriodModel, PerformanceRecord>(
                //        (performanceRecord, creator, nomination) =>
                //        {
                //            performanceRecord.Creator = creator;
                //            performanceRecord.NominationPeriod = nomination;

                //            return performanceRecord;
                //        },
                //         "EmployeeId, NominationPeriodId",
                //         criteria).ConfigureAwait(false);

                //return performanceRecords;
            }
        }
    }
}
