namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;

    public interface IProbationService
    {
        void SendMail();

        void SendNotification();

        Task SendSkypeMessage();

        Task AddNewEmployeeToProbation();
    }
}