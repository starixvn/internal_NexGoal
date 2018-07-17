namespace NexGoal.ViewModels.EOQY
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;
    using NexGoal.Models.EOQY;

    public class EOQYViewModel : EOQYModel
    {
        public string Title
        {
            get
            {
                var title = "EOY " + Year.ToString();

                if (Quarter != 0)
                {
                    switch (Quarter)
                    {
                        case 1:
                            title = "Employee of 1st Quarter, " + Year.ToString();
                            break;

                        case 2:
                            title = "Employee of 2nd Quarter, " + Year.ToString();
                            break;

                        case 3:
                            title = "Employee of 3rd Quarter, " + Year.ToString();
                            break;

                        default:
                            title = "Employee of 4th Quarter, " + Year.ToString();
                            break;
                    }
                }

                return title;
            }
        }

        public string QuarterToString
        {
            get
            {
                var quarter = string.Empty;

                switch (Quarter)
                {
                    case 1:
                        quarter = "1st Quarter";
                        break;

                    case 2:
                        quarter = "2nd Quarter";
                        break;

                    case 3:
                        quarter = "3rd Quarter";
                        break;

                    case 4:
                        quarter = "4th Quarter";
                        break;

                    default:
                        quarter = string.Empty;
                        break;
                }

                return quarter;
            }
        }

        public string StatusToString
        {
            get
            {
                return EnumHelper.GetDescription((NominationSettingStatus)StatusId);
            }
        }
    }
}