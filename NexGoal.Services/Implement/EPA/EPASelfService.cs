namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using NexGoal.Core.Enumerations;
    using NexGoal.Criteria.EmployeeEPA;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Services.Interfaces;

    public class EPASelfService : IEPASelfService
    {
        private readonly IEmployeeEPARepository _employeeEPARepository;

        public EPASelfService()
            : this(null)
        {
        }

        public EPASelfService(
            IEmployeeEPARepository employeeEPARepository)
        {
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
        }

        public IEnumerable<EPAModel> GetEPA(GetEPAByEmployeeCriteria criteria)
        {
            IEnumerable<EPAModel> epas = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEPA(criteria);

                foreach (EPAModel epa in epas)
                {
                    epa.Process.CurrentProcess = CopyEPAProcess(epa.Process.Self);
                }
            }

            return epas;
        }

        public EPAModel GetEPAInfo(GetEPAInfoByEmployeeCriteria criteria)
        {
            var epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess = CopyEPAProcess(epaInfo.Process.Self);
                SetMaxPerformanceScore(epaInfo);
                SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKPIModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationBySelf;
                }
            }

            return epaInfo;
        }

        private void SpreadKPIScore(IList<EPAKPIGroupModel> kpiGroups)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI && kpiGroup.Weight != null)
                {
                    double averageWeight = kpiGroup.Weight.Value / kpiGroup.KPIs.Count;

                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.Weight = averageWeight;
                    }
                }
            }
        }

        private void SetMaxPerformanceScore(EPAModel epaInfo)
        {
            switch (epaInfo.Process.CurrentProcess.ProcessId)
            {
                case (int)ProcessType.Self:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.SelfEvaluated;
                    break;

                case (int)ProcessType.Evaluator:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Evaluator;
                    break;

                case (int)ProcessType.Reviewer:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Reviewer;
                    break;
            }

            epaInfo.Process.Self.MaxPerformanceScore = epaInfo.ScoreRatingType.SelfEvaluated;

            if (epaInfo.HasDisciplineScore)
            {
                epaInfo.Process.Self.MaxPerformanceScore += epaInfo.MaxDisciplineScore;
                epaInfo.Process.CurrentProcess.MaxPerformanceScore += epaInfo.MaxDisciplineScore;
            }
        }

        private EPAProcessModel CopyEPAProcess(EPAProcessModel process)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, process);
                ms.Position = 0;

                return (EPAProcessModel)formatter.Deserialize(ms);
            }
        }
    }
}