namespace NexGoal.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Models.PRReaction;
    using NexGoal.Repositories.Interfaces;

    public class PerformanceRecordRepository : IPerformanceRecordRepository
    {
        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByEmployeeAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
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
                        "EmployeeId, EmployeeId, NominationPeriodId, PeriodId");

                    var projects = multi.Read<ProjectModel>();
                    var paSummaries = multi.Read<PASummaryModel>();
                    var paGradeDetails = multi.Read<PAGradeDetailModel>();

                    var reactions = multi.Read<PRReactionModel, Employee, PRReactionModel>(
                            (reaction, creator) =>
                            {
                                reaction.Creator = creator;
                                return reaction;
                            },
                            "EmployeeId");

                    var feedbacks = multi.Read<PRFeedbackModel, Employee, PRFeedbackModel>(
                        (feedback, creator) =>
                        {
                            feedback.Creator = creator;
                            return feedback;
                        },
                        "EmployeeId");

                    var feedbackReactions = multi.Read<PRFeedbackReactionModel, Employee, PRFeedbackReactionModel>(
                        (reaction, creator) =>
                        {
                            reaction.Creator = creator;
                            return reaction;
                        },
                        "EmployeeId");

                    foreach (var summary in paSummaries)
                    {
                        summary.GradeDetails = paGradeDetails.Where(x => x.PARecordId == summary.PARecordId);
                    }

                    foreach (PerformanceRecord performanceRecord in performanceRecords)
                    {
                        if (performanceRecord.TypeId == (int)PerformanceRecordType.SystemPerformanceAppraisal)
                        {
                            performanceRecord.PASummary = paSummaries.FirstOrDefault(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        }

                        performanceRecord.Projects = projects.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Reactions = reactions.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Feedbacks = feedbacks.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                    }

                    foreach (PRFeedbackModel feedback in feedbacks)
                    {
                        feedback.Reactions = feedbackReactions.Where(x => x.PRFeedbackId == feedback.PRFeedbackId);
                    }

                    return performanceRecords;
                }
            }
        }

        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceTimelineAsync(ICriteria criteria)
        {
            using (IObjectDb objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await objectDb.QueryAsync<PerformanceRecord, Employee, NominationPeriodModel, PerformanceRecord>(
                        (performanceRecord, creator, nomination) =>
                        {
                            performanceRecord.Creator = creator;
                            performanceRecord.NominationPeriod = nomination;

                            return performanceRecord;
                        },
                        "EmployeeId, NominationPeriodId",
                        criteria);
            }
        }

        public async Task<PerformanceRecord> GetPerformanceRecordByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceRecord = multi.Read<PerformanceRecord, Employee, Employee, PerformanceRecord>(
                        (record, employee, creator) =>
                        {
                            record.Creator = creator;
                            record.Employees = new List<Employee> { employee };

                            return record;
                        },
                        "EmployeeId, EmployeeId").FirstOrDefault();

                    if (performanceRecord != null)
                    {
                        performanceRecord.Projects = multi.Read<ProjectModel>();
                    }

                    return performanceRecord;
                }
            }
        }

        public async Task<PerformanceRecord> GetDraftPerformanceRecordByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceRecord = multi.Read<PerformanceRecord, Employee, PerformanceRecord>(
                        (record, creator) =>
                        {
                            record.Creator = creator;

                            return record;
                        },
                        "EmployeeId").FirstOrDefault();

                    if (performanceRecord != null)
                    {
                        performanceRecord.Projects = multi.Read<ProjectModel>();
                        performanceRecord.Employees = multi.Read<Employee>();
                    }

                    return performanceRecord;
                }
            }
        }

        public async Task<IEnumerable<PerformanceRecord>> GetPerformanceRecordByIdsAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceRecords = multi.Read<PerformanceRecord, Employee, Employee, PerformanceRecord>(
                        (performanceRecord, employee, creator) =>
                        {
                            performanceRecord.Employees = new List<Employee> { employee };
                            performanceRecord.Creator = creator;

                            return performanceRecord;
                        },
                        "EmployeeId, EmployeeId");
                    var projects = multi.Read<ProjectModel>();

                    foreach (PerformanceRecord performanceRecord in performanceRecords)
                    {
                        performanceRecord.Projects = projects.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                    }

                    return performanceRecords;
                }
            }
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceDetailAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceRecords = multi.Read<PerformanceRecord, Employee, NominationPeriodModel, PeriodBaseModel, PerformanceRecord>(
                        (performanceRecord, creator, nomination, period) =>
                        {
                            performanceRecord.Creator = creator;
                            performanceRecord.NominationPeriod = nomination;
                            performanceRecord.Period = period;

                            return performanceRecord;
                        },
                        "EmployeeId, NominationPeriodId, PeriodId");

                    var members = multi.Read<Employee>();
                    var projects = multi.Read<ProjectModel>();
                    var paSummaries = multi.Read<PASummaryModel>();
                    var paGradeDetails = multi.Read<PAGradeDetailModel>();

                    var reactions = multi.Read<PRReactionModel, Employee, PRReactionModel>(
                            (reaction, creator) =>
                            {
                                reaction.Creator = creator;
                                return reaction;
                            },
                            "EmployeeId");

                    var feedbacks = multi.Read<PRFeedbackModel, Employee, PRFeedbackModel>(
                        (feedback, creator) =>
                        {
                            feedback.Creator = creator;
                            return feedback;
                        },
                        "EmployeeId");

                    var feedbackReactions = multi.Read<PRFeedbackReactionModel, Employee, PRFeedbackReactionModel>(
                        (reaction, creator) =>
                        {
                            reaction.Creator = creator;
                            return reaction;
                        },
                        "EmployeeId");

                    foreach (var summary in paSummaries)
                    {
                        summary.GradeDetails = paGradeDetails.Where(x => x.PARecordId == summary.PARecordId);
                    }

                    foreach (PerformanceRecord performanceRecord in performanceRecords)
                    {
                        performanceRecord.Employees = members.Where(t => t.LinkObjectId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Projects = projects.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Reactions = reactions.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Feedbacks = feedbacks.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);

                        if (performanceRecord.TypeId == (int)PerformanceRecordType.SystemPerformanceAppraisal)
                        {
                            performanceRecord.PASummary = paSummaries.FirstOrDefault(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        }
                    }

                    foreach (PRFeedbackModel feedback in feedbacks)
                    {
                        feedback.Reactions = feedbackReactions.Where(x => x.PRFeedbackId == feedback.PRFeedbackId);
                    }

                    return performanceRecords;
                }
            }
        }

        public async Task<IEnumerable<PerformanceRecord>> GetMemberPerformanceAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceRecords = multi.Read<PerformanceRecord, Employee, PerformanceRecord>(
                        (performanceRecord, creator) =>
                        {
                            performanceRecord.Creator = creator;
                            return performanceRecord;
                        },
                        "EmployeeId");

                    var members = multi.Read<Employee>();
                    var projects = multi.Read<ProjectModel>();

                    var reactions = multi.Read<PRReactionModel, Employee, PRReactionModel>(
                            (reaction, creator) =>
                            {
                                reaction.Creator = creator;
                                return reaction;
                            },
                            "EmployeeId");

                    var feedbacks = multi.Read<PRFeedbackModel, Employee, PRFeedbackModel>(
                        (feedback, creator) =>
                        {
                            feedback.Creator = creator;
                            return feedback;
                        },
                        "EmployeeId");

                    var feedbackReactions = multi.Read<PRFeedbackReactionModel, Employee, PRFeedbackReactionModel>(
                        (reaction, creator) =>
                        {
                            reaction.Creator = creator;
                            return reaction;
                        },
                        "EmployeeId");

                    foreach (PerformanceRecord performanceRecord in performanceRecords)
                    {
                        performanceRecord.Employees = members.Where(t => t.LinkObjectId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Projects = projects.Where(t => t.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Reactions = reactions.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                        performanceRecord.Feedbacks = feedbacks.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                    }

                    foreach (PRFeedbackModel feedback in feedbacks)
                    {
                        feedback.Reactions = feedbackReactions.Where(x => x.PRFeedbackId == feedback.PRFeedbackId);
                    }

                    return performanceRecords;
                }
            }
        }

        public async Task<Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>> GetMemberPerformanceTimelineAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
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
                        "EmployeeId, EmployeeId, NominationPeriodId");

                    return new Tuple<IEnumerable<MemberPerformanceTimelineModel>, IEnumerable<PerformanceRecord>>(members, performanceRecords);
                }
            }
        }
    }
}