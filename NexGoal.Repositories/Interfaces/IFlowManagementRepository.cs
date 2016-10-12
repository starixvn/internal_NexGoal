namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.FlowManagement;
    using System.Collections.Generic;

    public interface IFlowManagementRepository
    {
        IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria);

        FlowTemplateModel GetFlowTemplateById(ICriteria criteria);

        IEnumerable<FlowModel> GetFlowManagement(ICriteria criteria);

        IEnumerable<DepartmentFlowModel> GetDepartmentFlow(ICriteria criteria);

        IEnumerable<PersonalFlowModel> GetPersonalFlow(ICriteria criteria);
    }
}