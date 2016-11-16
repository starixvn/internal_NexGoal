namespace NexGoal.Model.EmployeeEPA
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MonthlyEPAModel
    {
        public MonthlyEPAModel()
        {
            MonthlyEPAs = new List<EPAModel>();
        }

        public IList<EPAModel> MonthlyEPAs { get; set; }

        public double AverageSelfScore
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Self);
            }
        }

        public double AverageEvaluatorScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Evaluator);
            }
        }

        public double AverageReviewerScores
        {
            get
            {
                return CalculateAverageScore((int)ProcessType.Reviewer);
            }
        }

        public double AverageSelfScoreConverted { get; set; }

        public double AverageEvaluatorScoreConverted { get; set; }

        public double AverageReviewerScoreConverted { get; set; }

        private double CalculateAverageScore(int processType)
        {
            double average = 0;
            int count = 0;

            var finishedMonthlyEPAs = MonthlyEPAs.Where(x => x.Period.IsEndPeriod);
            foreach (EPAModel monthlyEPA in finishedMonthlyEPAs)
            {
                switch (processType)
                {
                    case (int)ProcessType.Self:
                        if (!string.IsNullOrEmpty(monthlyEPA.Process.Self.TotalScore))
                        {
                            count++;
                            average += float.Parse(monthlyEPA.Process.Self.TotalScore);
                        }

                        break;

                    case (int)ProcessType.Evaluator:
                        if (monthlyEPA.Process.AverageScoreByEvaluators > 0)
                        {
                            count++;
                            average += monthlyEPA.Process.AverageScoreByEvaluators;
                        }

                        break;

                    case (int)ProcessType.Reviewer:
                        if (monthlyEPA.Process.AverageScoreByReviewers > 0)
                        {
                            count++;
                            average += monthlyEPA.Process.AverageScoreByReviewers;
                        }

                        break;
                }
            }

            if (count > 0)
            {
                average = average / count;
                return (float)Math.Round(average, Constants.NumberOfDecimalDefault);
            }

            return average;
        }
    }
}