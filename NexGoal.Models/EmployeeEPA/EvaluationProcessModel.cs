namespace NexGoal.Model.EmployeeEPA
{
    using NexGoal.Core.Constants;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EvaluationProcessModel
    {
        public EvaluationProcessModel()
        {
            HumanResources = new List<EPAProcessModel>();
            Evaluators = new List<EPAProcessModel>();
            Reviewers = new List<EPAProcessModel>();
            Managements = new List<EPAProcessModel>();
        }

        public double AverageScoreByEvaluators
        {
            get
            {
                return CalculateAverageScore(Evaluators);
            }
        }

        public double AverageScoreByReviewers
        {
            get
            {
                return CalculateAverageScore(Reviewers);
            }
        }

        public double AverageScoreByManagements
        {
            get
            {
                return CalculateAverageScore(Managements);
            }
        }

        public double MaxPerformanceScore
        {
            get
            {
                double maxPerformanceScore = 0;

                if (Managements.Count > 0)
                {
                    maxPerformanceScore = Managements.First().MaxTotalScore;
                }
                else if (Reviewers.Count > 0)
                {
                    maxPerformanceScore = Reviewers.First().MaxTotalScore;
                }
                else if (Evaluators.Count > 0)
                {
                    maxPerformanceScore = Evaluators.First().MaxTotalScore;
                }

                return maxPerformanceScore;
            }
        }

        public double PerformanceScore
        {
            get
            {
                double performanceScore = AverageScoreByEvaluators;

                if (Managements.Count > 0)
                {
                    performanceScore = AverageScoreByManagements;
                }
                else if (Reviewers.Count > 0)
                {
                    performanceScore = AverageScoreByReviewers;
                }

                return performanceScore;
            }
        }

        public EPAProcessModel CurrentProcess { get; set; }

        public IList<EPAProcessModel> HumanResources { get; set; }

        public EPAProcessModel Self { get; set; }

        public IList<EPAProcessModel> Evaluators { get; set; }

        public IList<EPAProcessModel> Reviewers { get; set; }

        public IList<EPAProcessModel> Managements { get; set; }

        public double CalculateAverageScore(IList<EPAProcessModel> processes)
        {
            double average = 0.0;
            int count = 0;

            foreach (EPAProcessModel process in processes.Where(x => !string.IsNullOrEmpty(x.TotalScore)))
            {
                count++;
                average += Math.Round(double.Parse(process.TotalScore));
            }

            if (count > 0)
            {
                average = (double)Math.Round(average / count);
            }

            return average;
        }
    }
}