using NexGoal.Core.Constants;
using NexGoal.Core.Enumerations;
using NexGoal.Criteria.EmployeeEPA;
using NexGoal.Model.EmployeeEPA;
using NexGoal.Repositories.Implement;
using NexGoal.Repositories.Interfaces;
using NexGoal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace NexGoal.Services.Implement
{
    public class EPAReviewerService : IEPAReviewerService
    {
        private readonly IEmployeeEPARepository employeeEPARepository;
        private readonly IEmployeeEPAService employeeEPAService;

        public EPAReviewerService()
            : this(null, null)
        {
        }

        public EPAReviewerService(IEmployeeEPARepository employeeEPARepository,
            IEmployeeEPAService employeeEPAService)
        {
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPA(GetEPAByReviewerCriteria criteria)
        {
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = employeeEPARepository.GetEPA(criteria);
                string epaIds = string.Join(",", memberEPAs.Where(x => x.HasMonthlyEPA && x.Period.TargetTypeId == (int)TargetType.SemiAnnual).Select(x => x.EPAId));

                foreach (EPAModel memberEPA in memberEPAs)
                {
                    memberEPA.Process.CurrentProcess = CopyEPAProcess(memberEPA.Process.Reviewers.FirstOrDefault(x => x.EvaluatorId == criteria.ReviewerId));
                    SetMaxPerformanceScore(memberEPA);
                    SetMaxSubtractAndMaxAdditionScore(memberEPA);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfo(GetEPAInfoByReviewerCriteria criteria)
        {
            var epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.LastRating = employeeEPAService.GetLastRating(epaInfo.Employee.EmployeeId, epaInfo.Period.TargetTypeId);
                epaInfo.Process.CurrentProcess = CopyEPAProcess(epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorId == criteria.ReviewerId));
                SetMaxPerformanceScore(epaInfo);
                SpreadKPIScore(epaInfo.KPIGroups);
            }

            return epaInfo;
        }

        private void AveragedLastRating(IEnumerable<EvaluationModel> kpis)
        {
            foreach (var kpi in kpis)
            {
                var averagedRating = kpis.Where(i => i.KPIId == kpi.KPIId).Average(i => i.Rate);
                kpi.Rate = averagedRating;
            }

            kpis = kpis.Distinct();
        }

        private void SpreadKPIScore(IList<EPAKPIGroupModel> kpiGroups)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI && kpiGroup.Weight != null)
                {
                    double averageWeight = kpiGroup.Weight.Value / kpiGroup.KPIs.Count;
                    averageWeight = SplitNumber(averageWeight, Constants.NumberOfDecimals);

                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.Weight = averageWeight;
                    }
                }
            }
        }

        private double SplitNumber(double number, int numberOfDecimals)
        {
            double result = number;
            numberOfDecimals = numberOfDecimals <= 0 ? Constants.NumberOfDecimalDefault : numberOfDecimals;

            if ((number % 1) != 0)
            {
                int index = number.ToString().IndexOf('.');
                string naturalFraction = number.ToString().Substring(0, index),
                    decimalFraction = number.ToString().Substring(index + 1);

                result = double.Parse(naturalFraction + "." + decimalFraction.Substring(0, Math.Min(decimalFraction.Length, numberOfDecimals)));
            }

            return result;
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

        private void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo)
        {
            float maxAdditionScore = 0;

            switch (epaInfo.Process.CurrentProcess.ProcessId)
            {
                case (int)ProcessType.Reviewer:
                    if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                    {
                        epaInfo.Process.CurrentProcess.MaxSubtractScore = -epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                        maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore - epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                    }
                    else
                    {
                        epaInfo.Process.CurrentProcess.MaxSubtractScore = -epaInfo.Process.AverageScoreByEvaluators;
                        maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore - epaInfo.Process.AverageScoreByEvaluators;
                    }

                    epaInfo.Process.CurrentProcess.MaxAdditionScore = (float)Math.Round(maxAdditionScore, Constants.NumberOfDecimalDefault);
                    break;
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
