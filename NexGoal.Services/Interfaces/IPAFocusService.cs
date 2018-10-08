namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.PAFocus;

    public interface IPAFocusService
    {
        Task<IEnumerable<PAFocusModel>> GetPAFocusAsync(GetPAFocusParam param);
    }
}