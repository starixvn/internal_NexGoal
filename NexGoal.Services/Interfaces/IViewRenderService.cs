namespace NexGoal.Services.Interfaces
{
    public interface IViewRenderService
    {
        string Render<TModel>(string name, TModel model);
    }
}