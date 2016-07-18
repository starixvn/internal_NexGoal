namespace NexGoal.Web.Services
{
    using NexGoal.Core.Enumerations;

    public interface IWebCommonService
    {
        int GetPermission(string functionKey);

        bool HasPermissionOnFunction(string functionKey, Policy policy);
    }
}