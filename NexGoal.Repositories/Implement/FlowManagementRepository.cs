namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.FlowManagement;
    using NexGoal.Model.TargetType;
    using NexGoal.Repositories.Interfaces;

    public class FlowManagementRepository : IFlowManagementRepository
    {
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<FlowTemplateModel, TargetTypeModel, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria.GetSpParams());
            }
        }

        public FlowTemplateModel GetFlowTemplateById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<FlowTemplateModel, TargetTypeModel, FlowTemplateModel>(
                   (ft, tt) =>
                   {
                       ft.TargetType = tt;
                       return ft;
                   },
                   "TargetTypeId",
                   criteria.GetSpParams());
            }
        }

        public IEnumerable<FlowModel> GetFlowManagement(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<FlowModel, FlowTemplateModel, EmployeeModel, FlowModel>(
                   (f, ft, e) =>
                   {
                       f.FlowTemplate = ft;
                       f.EvaluatorInfo = e;
                       return f;
                   },
                   "FlowTemplateId, EmployeeId",
                   criteria.GetSpParams());
            }
        }

        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var flows = multi.Read<DepartmentFlowModel>();
                    var processes = multi.Read<ProcessFlowModel>();

                    GroupProcessFlowByDepartment(flows, processes);

                    return flows;
                }
            }
        }

        public IEnumerable<PersonalFlowModel> GetPersonalFlow(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var flows = multi.Read<PersonalFlowModel, Group, PersonalFlowModel>(
                        (flow, gr) =>
                        {
                            flow.Group = gr;

                            return flow;
                        },
                        "GroupId");

                    var processes = multi.Read<ProcessFlowModel>();

                    GroupProcessFlowByEmployee(flows, processes);

                    return flows;
                }
            }
        }

        private static void GroupProcessFlowByDepartment(IEnumerable<DepartmentFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            foreach (DepartmentFlowModel flow in flows)
            {
                foreach (ProcessFlowModel process in processes.Where(x => x.DepartmentId.Equals(flow.DepartmentId)))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            flow.HumanResources.Add(process);
                            break;

                        case (int)ProcessType.Evaluator:
                            flow.Evaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            flow.Reviewers.Add(process);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }

        private static void GroupProcessFlowByEmployee(IEnumerable<PersonalFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            foreach (PersonalFlowModel flow in flows)
            {
                foreach (ProcessFlowModel process in processes.Where(x => x.EmployeeId.Equals(flow.EmployeeId)))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.HumanResource:
                            flow.HumanResources.Add(process);
                            break;

                        case (int)ProcessType.Evaluator:
                            flow.Evaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            flow.Reviewers.Add(process);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }
    }
}