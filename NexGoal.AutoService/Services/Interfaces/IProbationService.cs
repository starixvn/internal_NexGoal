namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;

    public interface IProbationService
    {
        Task SendMailAsync();

        Task SendNotificationAsync();

        Task SendSkypeMessage();

        Task AddNewEmployeeToProbation();
    }
}